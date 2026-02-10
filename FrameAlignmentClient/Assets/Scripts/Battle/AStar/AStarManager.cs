using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FixedMathSharp;

public class AStarManager
{
    private static AStarManager _instance;
    public static AStarManager Instance => _instance ??= new AStarManager();

    public int Cols { get; private set; }
    public int Rows { get; private set; }
    public Fixed64 NodeSize { get; private set; }
    public Vector2d Origin { get; private set; }
    private bool[] _walkableData;

    private AStarManager() { }

    public void LoadMap(TextAsset mapFile)
    {
        if (mapFile == null) return;
        using (MemoryStream ms = new MemoryStream(mapFile.bytes))
        using (BinaryReader br = new BinaryReader(ms))
        {
            Cols = br.ReadInt32();
            Rows = br.ReadInt32();
            NodeSize = (Fixed64)((double)br.ReadSingle());
            Origin = new Vector2d((Fixed64)((double)br.ReadSingle()), (Fixed64)((double)br.ReadSingle()));

            _walkableData = new bool[Cols * Rows];
            for (int i = 0; i < _walkableData.Length; i++)
                _walkableData[i] = br.ReadBoolean();
        }
    }

    public bool IsWalkable(int x, int z)
    {
        if (x < 0 || x >= Cols || z < 0 || z >= Rows) return false;
        return _walkableData[z * Cols + x];
    }

    public (int x, int z) WorldToGrid(Vector2d worldPos)
    {
        int x = (int)((worldPos.x - Origin.x) / NodeSize);
        int z = (int)((worldPos.y - Origin.y) / NodeSize);
        return (Mathf.Clamp(x, 0, Cols - 1), Mathf.Clamp(z, 0, Rows - 1));
    }

    public Vector2d GridToWorld(int x, int z)
    {
        return new Vector2d(
            Origin.x + (Fixed64)x * NodeSize + NodeSize * Fixed64.Half,
            Origin.y + (Fixed64)z * NodeSize + NodeSize * Fixed64.Half
        );
    }

    // --- 核心寻路入口 ---
    public List<Vector2d> FindPath(Vector2d start, Vector2d end)
    {
        var startGrid = WorldToGrid(start);
        var endGrid = WorldToGrid(end);

        if (!IsWalkable(endGrid.x, endGrid.z)) return new List<Vector2d>();

        // [优化1] 如果起点和终点直接可见，直接返回直线路径
        if (IsStraightLineClear(start, end))
            return new List<Vector2d> { start, end };

        // 标准 A* 逻辑
        Node startNode = new Node(startGrid.x, startGrid.z) { GCost = Fixed64.Zero };
        startNode.HCost = CalculateHCost(startNode, endGrid);
        List<Node> openSet = new List<Node> { startNode };
        HashSet<(int, int)> closedSet = new HashSet<(int, int)>();

        while (openSet.Count > 0)
        {
            Node currentNode = openSet.OrderBy(n => n.FCost).First();
            openSet.Remove(currentNode);
            closedSet.Add((currentNode.X, currentNode.Z));

            if (currentNode.X == endGrid.x && currentNode.Z == endGrid.z)
            {
                List<Vector2d> rawPath = RetracePath(currentNode);
                // [核心改进] 这里的 rawPath 是格子路径，需要进行平滑处理
                return SimplifyPath(rawPath);
            }

            foreach (var neighborPos in GetNeighbors(currentNode))
            {
                if (closedSet.Contains(neighborPos)) continue;

                Fixed64 moveCost = currentNode.GCost + (neighborPos.x != currentNode.X && neighborPos.z != currentNode.Z ? (Fixed64)1.41421356 : Fixed64.One);
                Node neighborNode = openSet.FirstOrDefault(n => n.X == neighborPos.x && n.Z == neighborPos.z);

                if (neighborNode == null)
                {
                    neighborNode = new Node(neighborPos.x, neighborPos.z);
                    openSet.Add(neighborNode);
                }

                if (moveCost < neighborNode.GCost)
                {
                    neighborNode.GCost = moveCost;
                    neighborNode.HCost = CalculateHCost(neighborNode, endGrid);
                    neighborNode.Parent = currentNode;
                }
            }
        }
        return new List<Vector2d>();
    }

    // --- 路径平滑逻辑 (String Pulling / Funnel 思想简化版) ---
    private List<Vector2d> SimplifyPath(List<Vector2d> rawPath)
    {
        if (rawPath.Count <= 2) return rawPath;

        List<Vector2d> simplified = new List<Vector2d> { rawPath[0] };
        int current = 0;

        while (current < rawPath.Count - 1)
        {
            int bestNext = current + 1;
            // 贪心算法：从最远的点开始检查，看是否能直线到达
            for (int i = rawPath.Count - 1; i > current + 1; i--)
            {
                if (IsStraightLineClear(rawPath[current], rawPath[i]))
                {
                    bestNext = i;
                    break;
                }
            }
            simplified.Add(rawPath[bestNext]);
            current = bestNext;
        }
        return simplified;
    }

    // --- 定点数射线检测 (检测两点间是否有障碍) ---
    public bool IsStraightLineClear(Vector2d start, Vector2d end)
    {
        Fixed64 dist = Vector2d.Distance(start, end);
        if (dist < NodeSize) return true;

        Vector2d dir = end - start;
        dir.Normalize();

        // 步进检测，采样率设为半个格子宽
        Fixed64 step = NodeSize * Fixed64.Half;
        for (Fixed64 d = step; d < dist; d += step)
        {
            var grid = WorldToGrid(start + dir * d);
            if (!IsWalkable(grid.x, grid.z)) return false;
        }
        return true;
    }

    private IEnumerable<(int x, int z)> GetNeighbors(Node n)
    {
        for (int x = -1; x <= 1; x++)
            for (int z = -1; z <= 1; z++)
            {
                if (x == 0 && z == 0) continue;
                int nx = n.X + x;
                int nz = n.Z + z;
                if (IsWalkable(nx, nz)) yield return (nx, nz);
            }
    }

    private class Node
    {
        public int X; public int Z;
        public Fixed64 GCost; public Fixed64 HCost;
        public Fixed64 FCost => GCost + HCost;
        public Node Parent;
        public Node(int x, int z) { X = x; Z = z; GCost = Fixed64.MAX_VALUE; }
    }

    private Fixed64 CalculateHCost(Node node, (int x, int z) endGrid) =>
        FixedMath.Abs((Fixed64)node.X - (Fixed64)endGrid.x) + FixedMath.Abs((Fixed64)node.Z - (Fixed64)endGrid.z);

    private List<Vector2d> RetracePath(Node endNode)
    {
        List<Vector2d> path = new List<Vector2d>();
        Node curr = endNode;
        while (curr != null) { path.Add(GridToWorld(curr.X, curr.Z)); curr = curr.Parent; }
        path.Reverse();
        return path;
    }
}