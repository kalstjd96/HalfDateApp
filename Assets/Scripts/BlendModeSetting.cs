using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static BlendModeSetting;
using Time = UnityEngine.Time;

public class BlendModeSetting : MonoBehaviour
{


    #region Rendering Mode�� �ٲٱ� ���� ����� �ڵ�
    // Material ������ ���� ����ü
    public struct MaterialInfo
    {
        public Material[] materials;       // Material �迭
        public MeshRenderer renderer;      // MeshRenderer
        public BlendMode[] modes;          // Rendering Mode �迭

        public MaterialInfo(Material[] materials, MeshRenderer renderer, BlendMode[] modes)
        {
            this.materials = materials;
            this.renderer = renderer;
            this.modes = modes;
        }
    }

    public enum BlendMode
    {
        Opaque,
        Cutout,
        Fade,
        Transparent
    }

    #endregion

    MaterialInfo[] materialInfos;
    public Transform player; // Player ������Ʈ�� Transform ������Ʈ
    public float raycastDistance = 10f; // ����ĳ��Ʈ �˻� �Ÿ�

    public GameObject[] testArray;

    private void Start()
    {
        On(testArray, 0.5f);
    }


    public static void ChangeRenderMode(Material standardShaderMaterial, BlendMode blendMode)
    {
        switch (blendMode)
        {
            case BlendMode.Opaque:
                standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                standardShaderMaterial.SetInt("_ZWrite", 1);
                standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                standardShaderMaterial.renderQueue = -1;
                break;
            case BlendMode.Cutout:
                standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                standardShaderMaterial.SetInt("_ZWrite", 1);
                standardShaderMaterial.EnableKeyword("_ALPHATEST_ON");
                standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                standardShaderMaterial.renderQueue = 2450;
                break;
            case BlendMode.Fade:
                standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                standardShaderMaterial.SetInt("_ZWrite", 0);
                standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                standardShaderMaterial.EnableKeyword("_ALPHABLEND_ON");
                standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                standardShaderMaterial.renderQueue = 3000;
                break;
            case BlendMode.Transparent:
                standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                standardShaderMaterial.SetInt("_ZWrite", 0);
                standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                standardShaderMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                standardShaderMaterial.renderQueue = 3000;
                break;
        }

    }

    //private void Update()
    //{
    //    // Player�� ī�޶� ������ ������ ���մϴ�.
    //    Vector3 directionToPlayer = player.position - transform.position;

    //    transform.LookAt(player);
    //    RaycastHit hit;
    //    if (Physics.Raycast(transform.position, directionToPlayer, out hit, raycastDistance))
    //    {
    //        // ����ĳ��Ʈ�� �浹�� ������Ʈ�� �±׸� Ȯ������ �ʰ� ��� ������Ʈ�� ���� �޼��带 ȣ���մϴ�.
    //        On(new GameObject[] { hit.collider.gameObject }, 0.5f);
    //    }
    //    else
    //    {
    //        // ����ĳ��Ʈ�� �浹���� ���� ���, Player�� ������ ���� �� ���� ó���� �����մϴ�.
    //        Off();
    //    }
    //}


    // Player�� �ǹ��� ������ ��
    public void On(GameObject[] objects, float transparency)
    {
        materialInfos = new MaterialInfo[objects.Length];

        for (int i = 0; i < materialInfos.Length; i++)
        {
            MeshRenderer renderer = objects[i].GetComponent<MeshRenderer>();
            Material[] materials = renderer.materials;
            BlendMode[] modes = new BlendMode[materials.Length];

            for (int j = 0; j < materials.Length; j++)
            {
                modes[j] = GetMaterialRenderingMode(materials[j]);
                ChangeRenderMode(materials[j], BlendMode.Transparent);

                // ���� ����
                Color materialColor = materials[j].color;
                materialColor.a = transparency;
                materials[j].color = materialColor;
            }

            materialInfos[i] = new MaterialInfo(materials, renderer, modes);
        }
    }

    // ���׸����� Rendering Mode ���� ��� �Լ�
    public BlendMode GetMaterialRenderingMode(Material material)
    {
        if (material == null)
        {
            //Debug.LogError("Material is null!");
            return BlendMode.Opaque; // �⺻������ Opaque�� ��ȯ�մϴ�.
        }

        // Material�� renderQueue ���� �����ɴϴ�.
        int renderQueue = material.renderQueue;

        // renderQueue ���� ������� Rendering Mode�� �Ǻ��մϴ�.
        if (renderQueue == (int)UnityEngine.Rendering.RenderQueue.Geometry)
        {
            return BlendMode.Opaque;
        }
        else if (renderQueue == (int)UnityEngine.Rendering.RenderQueue.AlphaTest)
        {
            return BlendMode.Cutout;
        }
        else if (renderQueue == (int)UnityEngine.Rendering.RenderQueue.Transparent)
        {
            return BlendMode.Transparent;
        }
        else
        {
            return BlendMode.Fade;
        }
    }

    // Player�� ������ ���� �� ����
    public void Off()
    {
        if (materialInfos == null)
        {
            //Debug.LogError("MaterialInfo array is null!");
            return;
        }

        foreach (MaterialInfo info in materialInfos)
        {
            if (info.renderer == null || info.materials == null || info.modes == null)
            {
                Debug.LogError("MaterialInfo is not properly initialized!");
                continue;
            }

            for (int i = 0; i < info.materials.Length; i++)
            {
                if (info.materials[i] == null)
                {
                    Debug.LogError("Material or BlendMode is null!");
                    continue;
                }

                ChangeRenderMode(info.materials[i], info.modes[i]);
            }
        }
    }
}