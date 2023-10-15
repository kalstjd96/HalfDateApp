using UnityEditor;
using UnityEngine;

public class OcclusionCullingSetup
{
    [MenuItem("MyMenu/Setup Occlusion Culling _%r")]
    private static void SetupOcclusionCulling()
    {
        // ī�޶� ������Ʈ�� Occlusion Culling �÷��� Ȱ��ȭ
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.useOcclusionCulling = true;
            Debug.Log("Main camera's occlusion culling enabled.");
        }

        // ���õ� ������Ʈ ��������
        GameObject selectedObject = Selection.activeGameObject;

        if (selectedObject != null)
        {
            // ���õ� ������Ʈ�� ��� ���� ������Ʈ ó��
            ProcessChildrenRecursively(selectedObject.transform);

            Debug.Log($"Processed all children of {selectedObject.name} for occlusion culling.");
        }
    }

    private static void ProcessChildrenRecursively(Transform parent)
    {
        foreach (Renderer child in parent.GetComponentsInChildren<Renderer>())
        {
            Renderer childRenderer = child;

            if (child.gameObject.layer == LayerMask.NameToLayer("dynamicObj"))
            {
                // Dynamic Occlusion �÷��� Ȱ��ȭ 
                // �ش� �ɼ��� Unity 2020 �̻� ���������� �����մϴ�.
                if (childRenderer != null)
                    childRenderer.allowOcclusionWhenDynamic = true;

                Debug.Log($"Enabled dynamic occlusion for {child.name}");

            }

            else if (child.gameObject.layer == LayerMask.NameToLayer("staticObj"))
            {
                GameObjectUtility.SetStaticEditorFlags(child.gameObject, StaticEditorFlags.OccluderStatic | StaticEditorFlags.OccludeeStatic);

                Debug.Log($"Set static flags for {child.name}");

            }
        }
    }
}
