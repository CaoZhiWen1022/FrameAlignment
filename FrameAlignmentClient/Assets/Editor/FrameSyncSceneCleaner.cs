using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class FrameSyncSceneCleaner : EditorWindow
{
    [MenuItem("Tools/FrameSync/Clean Scene Physics")]
    public static void CleanPhysics()
    {
        // 1. 查找场景中所有的 Collider
        Collider[] allColliders = GameObject.FindObjectsOfType<Collider>();
        int colliderCount = allColliders.Length;

        foreach (var col in allColliders)
        {
            // 如果你之后需要根据这些 Collider 烘焙定点数地图，
            // 建议在这里先调用你的导出逻辑。
            Undo.DestroyObjectImmediate(col);
        }

        // 2. 查找所有的 Rigidbody
        Rigidbody[] allRigidbodies = GameObject.FindObjectsOfType<Rigidbody>();
        int rbCount = allRigidbodies.Length;
        foreach (var rb in allRigidbodies)
        {
            Undo.DestroyObjectImmediate(rb);
        }

        // 3. 移除 NavMesh 相关
        UnityEngine.AI.NavMeshObstacle[] allObstacles = GameObject.FindObjectsOfType<UnityEngine.AI.NavMeshObstacle>();
        int navCount = allObstacles.Length;
        foreach (var nav in allObstacles)
        {
            Undo.DestroyObjectImmediate(nav);
        }

        Debug.Log($"<color=green>清洗完成！</color> 移除了 {colliderCount} 个Collider, {rbCount} 个Rigidbody, {navCount} 个导航组件。");
    }
}