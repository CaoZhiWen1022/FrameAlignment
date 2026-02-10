using FixedMathSharp;
using Proto;
using System;
using UnityEngine;

public class PlayerLogicState_Atk : PlayerLogicState_Base
{

    long targetUid;
    long startFrameIndex;

    public PlayerLogicState_Atk(PlayerLogicState state, LogicData logicData, Action<PlayerLogicState, object> action) : base(state, logicData, action)
    {
    }

    public override void OnEnter(StateMachine machine, IState prevState, object param)
    {
        targetUid = (long)(param);
        startFrameIndex = FrameSyncMgr.ins.CurFrameIndex;
        //修正朝向
        UpdateRotation();
    }

    public override void OnLeave(IState nextState, object param)
    {
        base.OnLeave(nextState, param);
    }

    public override void OnUpdate()
    {
        //固定帧后退出
        long diff = FrameSyncMgr.ins.CurFrameIndex - startFrameIndex;
        Fixed64 diff64 = (Fixed64)diff;
        if (diff64 == logicData.playerData.atkFrameCount)
        {
            SwitchState(PlayerLogicState.idle);
        }
        else if (diff64 == logicData.playerData.atkHarmFrame)
        {
            Debug.Log("创建子弹");
            HeroCfg heroCfg = ConfigMgr.heroCfg.GetHeroById(logicData.playerData.heroId);
            if (heroCfg.commonAtkType == CommonAtkType._远程)
            {
                GameEventMgr.Instance.EmitWithParam<object>(GameEventType._角色发射普工_参_, (playerUid: uid, targetUid: targetUid));
            }
            else
            {
                //直接造成伤害
                FrameSyncMgr.ins.OnBulletHit(targetUid, (Fixed64)200);
            }
        }
    }

    void UpdateRotation()
    {
        var curPos = logicData.posV3;
        var targetPos = FrameSyncMgr.ins.GetPlayerPosByUid(targetUid);
        Vector3d moveDir = (targetPos - curPos).Normalize();
        double dx = (double)moveDir.x;
        double dz = (double)moveDir.z;
        if (Math.Abs(dx) > 0.0001 || Math.Abs(dz) > 0.0001)
        {
            double rad = Math.Atan2(dx, dz);
            logicData.rotate.y = (Fixed64)(rad * (180 / Math.PI));
        }
    }

    public override bool IsAllowSwitch(uint target)
    {
        if (target == (uint)PlayerLogicState.atk) return false;
        //固定帧后退出
        long diff = FrameSyncMgr.ins.CurFrameIndex - startFrameIndex;
        Fixed64 diff64 = (Fixed64)diff;
        if (diff64 >= (Fixed64)10)
        {
            return true;
        }
        return false;
    }
}
