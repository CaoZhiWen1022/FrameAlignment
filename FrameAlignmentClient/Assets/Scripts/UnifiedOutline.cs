using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class UnifiedOutline : MonoBehaviour
{
    public Material outlineMaterial; // 使用上面的 Shader
    private CommandBuffer cb;

    void OnEnable()
    {
        if (outlineMaterial == null) return;

        cb = new CommandBuffer();
        cb.name = "Unified Outline";

        // 获取该物体及所有子物体的 Renderer
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        // 核心逻辑：使用 Stencil 保证不覆盖模型本身
        int stencilID = Shader.PropertyToID("_StencilRef");

        // 1. 先让模型所有零件写模板缓冲区
        foreach (var r in renderers)
        {
            cb.DrawRenderer(r, r.sharedMaterial, 0, 0);
        }

        // 2. 只有不在模型范围内的地方才画橙色
        // (注：为了简单起见，这里直接用 DrawRenderer 的扩展 Pass)
        // 这一步建议配合 Post-Processing 使用，但如果你想立刻看到效果：
        // 我们可以直接在 Renderer 上增加一个 Material 数组
    }
}