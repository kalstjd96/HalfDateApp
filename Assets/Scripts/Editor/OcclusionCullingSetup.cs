using UnityEditor;
using UnityEngine;

public class OcclusionCullingSetup
{
    [MenuItem("MyMenu/Setup Occlusion Culling _%r")]
    private static void SetupOcclusionCulling()
    {
        // 카메라 컴포넌트의 Occlusion Culling 플래그 활성화
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.useOcclusionCulling = true;
            Debug.Log("Main camera's occlusion culling enabled.");
        }

        // 선택된 오브젝트 가져오기
        GameObject selectedObject = Selection.activeGameObject;

        if (selectedObject != null)
        {
            // 선택된 오브젝트의 모든 하위 오브젝트 처리
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
                // Dynamic Occlusion 플래그 활성화 
                // 해당 옵션은 Unity 2020 이상 버전에서만 지원합니다.
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
