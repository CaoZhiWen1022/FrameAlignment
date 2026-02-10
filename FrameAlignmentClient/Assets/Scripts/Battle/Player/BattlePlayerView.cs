using Assets.Scripts.FGUI;
using Battle;
using FairyGUI;
using FixedMathSharp;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 角色渲染层
/// </summary>
public class BattlePlayerView : MonoBehaviour
{
    /// <summary>
    /// 角色逻辑层实例
    /// </summary>
    private BattlePlayerLogic playerLogic;

    /// <summary>
    /// 平滑移动速度（越大越灵敏，建议5-15之间）
    /// </summary>
    [Header("平滑移动配置")]
    [SerializeField] private float smoothSpeed = 10f;

    /// <summary>
    /// SmoothDamp用的速度缓存（内部插值用）
    /// </summary>
    private Vector3 _velocity = Vector3.zero;

    /// <summary>
    /// 位置同步阈值（小于该值直接对齐，避免抖动）
    /// </summary>
    private const float PosSyncThreshold = 0.001f;

    public BattlePlayerAniController playerAni;

    private BattlePanel battleUI;
    private UI_PlayerHpBar hpBar;

    private List<UI_HarmValue> harmValues = new List<UI_HarmValue>();

    private void Awake()
    {
        playerAni = transform.GetChild(0).AddComponent<BattlePlayerAniController>();
    }

    public void Init(BattlePlayerLogic logic)
    {
        playerLogic = logic;
        // 初始化时直接对齐逻辑层初始位置，避免初始偏移
        if (playerLogic != null)
        {
            SyncPosImmediately();
        }
        //注册状态切换回调
        playerLogic.SetStateSwitchHandler(LogicStateSwitchHandler);
        //注册伤害回调
        playerLogic.SetTakeHarmHandler(TakeHarmHandler);

        //初始化血条组件
        battleUI = GameUIFrame.Instance.uiFrame.GetUIInstance((int)UIID.BattlePanel) as BattlePanel;
        if (logic.playerInfo.UserId == AuthSocket.userInfo.UserId)
        {
            hpBar = battleUI.ui.m_thisHpBar;
        }
        else
        {
            hpBar = battleUI.ui.m_enemyHpBar;
        }
    }

    void TakeHarmHandler(Fixed64 value)
    {
        UI_HarmValue harmValue = UI_HarmValue.CreateInstance();
        battleUI.ui.m_harmValueLayer.AddChild(harmValue);
        harmValues.Add(harmValue);
        harmValue.m_value.text = value.ToString();
        Vector3 worldPos = transform.position;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        screenPos.y = Screen.height - screenPos.y;
        Vector2 pt = GRoot.inst.GlobalToLocal(screenPos);
        harmValue.SetXY(pt.x - 50, pt.y - 50);
        harmValue.m_t0.Play(() =>
        {
            harmValue.Dispose();
            harmValues.Remove(harmValue);
        });
    }

    void LogicStateSwitchHandler(PlayerLogicState state)
    {
        switch (state)
        {

            case PlayerLogicState.idle: playerAni.SetAni(BattlePlayerAniName.idle); break;
            case PlayerLogicState.atkMove:
            case PlayerLogicState.move: playerAni.SetAni(BattlePlayerAniName.run); break;
            case PlayerLogicState.atk: playerAni.SetAni(BattlePlayerAniName.atk); break;
            case PlayerLogicState.die:
                {
                    playerAni.SetAni(BattlePlayerAniName.death);
                    //需要屏蔽碰撞组件防止被点击
                    transform.GetChild(0).GetComponent<BoxCollider>().enabled = false;
                    break;
                }
            case PlayerLogicState.revive:
                {
                    playerAni.SetAni(BattlePlayerAniName.idle);
                    //复活时恢复碰撞组件
                    if (playerLogic.playerInfo.UserId != AuthSocket.userInfo.UserId)
                    {
                        transform.GetChild(0).GetComponent<BoxCollider>().enabled = true;
                    }
                    break;
                }
        }
    }

    private void Update()
    {
        SmoothSyncLogicPos();
        UpdateRotate();
        UpdateHpBar();
        UpdateHarmValuesPos();
    }

    private void UpdateHarmValuesPos()
    {
        Vector3 worldPos = transform.position;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        screenPos.y = Screen.height - screenPos.y;
        Vector2 pt = GRoot.inst.GlobalToLocal(screenPos);
        foreach (var item in harmValues)
        {
            item.SetXY(pt.x - 50, pt.y - 50);
        }
    }

    private void UpdateHpBar()
    {
        //更新位置
        Vector3 worldPos = transform.position;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        //原点位置转换
        screenPos.y = Screen.height - screenPos.y;
        Vector2 pt = GRoot.inst.GlobalToLocal(screenPos);
        hpBar.SetXY(pt.x - (hpBar.width / 2), pt.y - 150);
        //更新血量
        hpBar.max = (int)playerLogic.logicData.playerData.maxHp;
        hpBar.value = (int)playerLogic.logicData.playerData.hp;
    }

    private void UpdateRotate()
    {
        Vector3 v3 = new Vector3((float)playerLogic.Rotate.x, (float)playerLogic.Rotate.y, (float)playerLogic.Rotate.z);
        Quaternion quaternion = Quaternion.Euler(v3);
        transform.rotation = Quaternion.Lerp(transform.rotation, quaternion, 0.1f);
    }

    /// <summary>
    /// 平滑同步逻辑层位置到视图
    /// </summary>
    private void SmoothSyncLogicPos()
    {
        // 空值保护
        if (playerLogic == null) return;

        // 逻辑层位置转Unity浮点坐标
        Vector3 targetPos = new Vector3(
            (float)playerLogic.PosV3.x,
            (float)playerLogic.PosV3.y,
            (float)playerLogic.PosV3.z
        );

        // 距离过小时直接对齐，避免平滑抖动
        if (Vector3.Distance(transform.position, targetPos) < PosSyncThreshold)
        {
            transform.position = targetPos;
            _velocity = Vector3.zero; // 重置速度缓存
            //playerAni.SetAni(BattlePlayerAniName.idle);
            return;
        }

        // 使用SmoothDamp实现带阻尼的平滑移动
        transform.position = Vector3.SmoothDamp(
            transform.position,    // 当前位置
            targetPos,             // 目标位置（逻辑层计算后的位置）
            ref _velocity,         // 速度缓存（必须ref）
            1f / smoothSpeed,      // 平滑时间（速度的倒数，方便调参）
            Mathf.Infinity,        // 最大速度（无限制）
            Time.deltaTime         // 帧时间
        );
    }

    /// <summary>
    /// 立即同步位置（初始化/场景切换时用）
    /// </summary>
    public void SyncPosImmediately()
    {
        if (playerLogic == null) return;
        transform.position = new Vector3(
            (float)playerLogic.PosV3.x,
            (float)playerLogic.PosV3.y,
            (float)playerLogic.PosV3.z
        );
        _velocity = Vector3.zero;
    }
}