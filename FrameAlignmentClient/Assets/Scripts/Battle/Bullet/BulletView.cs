using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BulletView : MonoBehaviour
{
    BulletLogic logic;

    /// <summary>
    /// 平滑移动速度（越大越灵敏，建议5-15之间）
    /// </summary>
    [Header("平滑移动配置")]
    [SerializeField] private float smoothSpeed = 20f;

    /// <summary>
    /// SmoothDamp用的速度缓存（内部插值用）
    /// </summary>
    private Vector3 _velocity = Vector3.zero;

    /// <summary>
    /// 位置同步阈值（小于该值直接对齐，避免抖动）
    /// </summary>
    private const float PosSyncThreshold = 0.001f;


    public void Init(BulletLogic logic)
    {
        this.logic = logic;
        SyncPosImmediately();
    }
    void Update()
    {
        if (gameObject.IsDestroyed()) return;
        if (logic.isDispose)
        {
            Destroy(this.gameObject);
            return;
        }
        //位置更新
        SmoothSyncLogicPos();
    }

    /// <summary>
    /// 平滑同步逻辑层位置到视图
    /// </summary>
    private void SmoothSyncLogicPos()
    {
        // 空值保护
        if (logic == null) return;


        Vector3 targetPos = new Vector3(
            (float)logic.posV3.x,
            (float)logic.posV3.y,
            (float)logic.posV3.z
        );


        if (Vector3.Distance(transform.position, targetPos) < PosSyncThreshold)
        {
            transform.position = targetPos;
            _velocity = Vector3.zero; 
            return;
        }
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPos,
            ref _velocity,
            1f / smoothSpeed,
            Mathf.Infinity,
            Time.deltaTime
        );
    }

    /// <summary>
    /// 立即同步位置（初始化/场景切换时用）
    /// </summary>
    public void SyncPosImmediately()
    {
        if (logic == null) return;
        transform.position = new Vector3(
            (float)logic.posV3.x,
            (float)logic.posV3.y,
            (float)logic.posV3.z
        );
        _velocity = Vector3.zero;
    }
}
