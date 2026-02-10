using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class MissingScriptCleaner : EditorWindow
{
    [MenuItem("Tools/清理场景遗失脚本")]
    public static void ShowWindow()
    {
        GetWindow<MissingScriptCleaner>("脚本清理器");
    }

    private void OnGUI()
    {
        GUILayout.Label("移除当前场景中所有遗失的脚本组件", EditorStyles.boldLabel);

        if (GUILayout.Button("开始扫描并清理", GUILayout.Height(40)))
        {
            CleanMissingScripts();
        }
    }

    private static void CleanMissingScripts()
    {
        // 获取当前活动场景
        Scene currentScene = SceneManager.GetActiveScene();
        GameObject[] rootObjects = currentScene.GetRootGameObjects();

        int totalRemoved = 0;
        int totalObjectsProcessed = 0;

        // 遍历所有根对象及其子物体
        foreach (var obj in rootObjects)
        {
            totalRemoved += RecursiveCleanup(obj);
        }

        // 标记场景为“已修改”，确保可以保存
        if (totalRemoved > 0)
        {
            EditorSceneManager.MarkSceneDirty(currentScene);
            Debug.Log($"清理完成！共从场景中移除了 {totalRemoved} 个遗失脚本。");
        }
        else
        {
            Debug.Log("未发现遗失脚本。场景很干净！");
        }

        EditorUtility.DisplayDialog("清理结果", $"共处理对象: {rootObjects.Length} (含子物体)\n移除遗失脚本数量: {totalRemoved}", "确定");
    }

    private static int RecursiveCleanup(GameObject obj)
    {
        // 移除该对象上所有缺失的脚本
        // GameObjectUtility.RemoveMonoBehavioursWithMissingScript 是 Unity 内置的高效清理方法
        int count = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(obj);

        // 递归处理子物体
        foreach (Transform child in obj.transform)
        {
            count += RecursiveCleanup(child.gameObject);
        }

        return count;
    }
}