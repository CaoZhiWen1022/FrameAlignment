using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class GridMapBaker : EditorWindow
{
    public GameObject walkableRoot;
    public GameObject obstacleRoot;
    public float nodeSize = 0.5f;
    public string savePath = "Assets/Resources/MapData.bytes";

    [MenuItem("Tools/地图网格烘焙器")]
    public static void ShowWindow() => GetWindow<GridMapBaker>("地图网格烘焙器");

    void OnGUI()
    {
        walkableRoot = (GameObject)EditorGUILayout.ObjectField("可行走根节点", walkableRoot, typeof(GameObject), true);
        obstacleRoot = (GameObject)EditorGUILayout.ObjectField("障碍物根节点", obstacleRoot, typeof(GameObject), true);
        nodeSize = EditorGUILayout.FloatField("网格精度 (Node Size)", nodeSize);
        savePath = EditorGUILayout.TextField("保存路径", savePath);

        if (GUILayout.Button("1. 生成网格数据 (Export Data)")) BakeData();
        if (GUILayout.Button("2. 生成可视化网格 (Debug View)")) BakeVisuals();
    }

    // --- 核心扫描逻辑 ---
    private List<bool> ScanGrid(out Bounds bounds, out int cols, out int rows)
    {
        // 1. 计算总边界 (基于所有 Walkable 面片)
        Renderer[] renderers = walkableRoot.GetComponentsInChildren<Renderer>();
        bounds = renderers[0].bounds;
        foreach (var r in renderers) bounds.Encapsulate(r.bounds);

        cols = Mathf.FloorToInt(bounds.size.x / nodeSize);
        rows = Mathf.FloorToInt(bounds.size.z / nodeSize);
        List<bool> grid = new List<bool>();

        // 2. 采样
        for (int z = 0; z < rows; z++)
        {
            for (int x = 0; x < cols; x++)
            {
                Vector3 checkPos = new Vector3(
                    bounds.min.x + x * nodeSize + nodeSize * 0.5f,
                    bounds.center.y, // 在面片高度附近检测
                    bounds.min.z + z * nodeSize + nodeSize * 0.5f
                );

                // 使用 Physics 判定（确保你的面片有 MeshCollider 且 Layer 设置正确）
                // 也可以直接用 Bounds.Contains 逻辑判定更轻量
                bool isWalkable = IsPointInRoot(checkPos, walkableRoot);
                bool isObstacle = IsPointInRoot(checkPos, obstacleRoot);

                grid.Add(isWalkable && !isObstacle);
            }
        }
        return grid;
    }

    private bool IsPointInRoot(Vector3 point, GameObject root)
    {
        if (root == null) return false;
        Renderer[] rs = root.GetComponentsInChildren<Renderer>();
        foreach (var r in rs)
        {
            // 简单判定：点是否在面片的 Bounds 内 (XZ平面)
            if (r.bounds.Contains(new Vector3(point.x, r.bounds.center.y, point.z))) return true;
        }
        return false;
    }

    // --- 功能 1：生成数据 ---
    void BakeData()
    {
        var grid = ScanGrid(out Bounds b, out int c, out int r);
        using (FileStream fs = new FileStream(savePath, FileMode.Create))
        using (BinaryWriter bw = new BinaryWriter(fs))
        {
            bw.Write(c); // 宽度
            bw.Write(r); // 高度
            bw.Write(nodeSize);
            bw.Write(b.min.x); // 起始坐标X
            bw.Write(b.min.z); // 起始坐标Z
            foreach (bool val in grid) bw.Write(val);
        }
        AssetDatabase.Refresh();
        Debug.Log($"数据已导出至: {savePath} | 尺寸: {c}x{r}");
    }

    // --- 功能 2：生成可视化 ---
    void BakeVisuals()
    {
        GameObject parent = new GameObject("Debug_Grid_Root");
        var grid = ScanGrid(out Bounds b, out int c, out int r);

        // 创建一个临时材质，专门给可视化用，避免修改默认材质
        Material debugMat = new Material(Shader.Find("Transparent/Diffuse"));
        debugMat.color = new Color(0, 1, 0, 0.4f);

        for (int i = 0; i < grid.Count; i++)
        {
            if (!grid[i]) continue;

            int x = i % c;
            int z = i / c;
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Quad);
            cube.transform.SetParent(parent.transform);
            cube.transform.position = new Vector3(b.min.x + x * nodeSize + nodeSize * 0.5f, b.min.y + 0.1f, b.min.z + z * nodeSize + nodeSize * 0.5f);
            cube.transform.rotation = Quaternion.Euler(90, 0, 0);
            cube.transform.localScale = Vector3.one * nodeSize * 0.9f;

            // 使用独立材质实例
            cube.GetComponent<Renderer>().material = debugMat;

            // 这样即使你删除了物体，debugMat 也会被销毁，不会影响全局默认材质
        }
    }
}