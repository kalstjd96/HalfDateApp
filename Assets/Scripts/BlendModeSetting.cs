using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static BlendModeSetting;
using Time = UnityEngine.Time;

public class BlendModeSetting : MonoBehaviour
{


    #region Rendering Mode를 바꾸기 위해 사용한 코드
    // Material 정보를 담을 구조체
    public struct MaterialInfo
    {
        public Material[] materials;       // Material 배열
        public MeshRenderer renderer;      // MeshRenderer
        public BlendMode[] modes;          // Rendering Mode 배열

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
    public Transform player; // Player 오브젝트의 Transform 컴포넌트
    public float raycastDistance = 10f; // 레이캐스트 검사 거리

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
    //    // Player와 카메라 사이의 방향을 구합니다.
    //    Vector3 directionToPlayer = player.position - transform.position;

    //    transform.LookAt(player);
    //    RaycastHit hit;
    //    if (Physics.Raycast(transform.position, directionToPlayer, out hit, raycastDistance))
    //    {
    //        // 레이캐스트에 충돌한 오브젝트의 태그를 확인하지 않고 모든 오브젝트에 대해 메서드를 호출합니다.
    //        On(new GameObject[] { hit.collider.gameObject }, 0.5f);
    //    }
    //    else
    //    {
    //        // 레이캐스트에 충돌하지 않은 경우, Player를 가리지 않을 때 복구 처리를 수행합니다.
    //        Off();
    //    }
    //}


    // Player가 건물에 가려질 때
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

                // 투명도 조절
                Color materialColor = materials[j].color;
                materialColor.a = transparency;
                materials[j].color = materialColor;
            }

            materialInfos[i] = new MaterialInfo(materials, renderer, modes);
        }
    }

    // 메테리얼의 Rendering Mode 값을 얻는 함수
    public BlendMode GetMaterialRenderingMode(Material material)
    {
        if (material == null)
        {
            //Debug.LogError("Material is null!");
            return BlendMode.Opaque; // 기본값으로 Opaque를 반환합니다.
        }

        // Material의 renderQueue 값을 가져옵니다.
        int renderQueue = material.renderQueue;

        // renderQueue 값을 기반으로 Rendering Mode를 판별합니다.
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

    // Player를 가리지 않을 때 복구
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