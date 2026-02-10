using FixedMathSharp;
using Proto;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerLogicState_Die : PlayerLogicState_Base
{
    private long dieStartFrame;


    public PlayerLogicState_Die(PlayerLogicState state, LogicData logicData, Action<PlayerLogicState, object> action) : base(state, logicData, action)
    {
    }

    public override void OnEnter(StateMachine machine, IState prevState, object param)
    {
        dieStartFrame = FrameSyncMgr.ins.CurFrameIndex;
    }

    public override void OnLeave(IState nextState, object param)
    {

    }

    public override void OnUpdate()
    {
        //检测是否该复活了
        if ((Fixed64)(FrameSyncMgr.ins.CurFrameIndex - dieStartFrame) == ConfigMgr.battleCfg.reviveFrameCount)
        {
            //进入复活状态
            SwitchState(PlayerLogicState.revive);
        }
    }


    public override bool IsAllowSwitch(uint target)
    {
        if (target == (uint)PlayerLogicState.revive && (Fixed64)(FrameSyncMgr.ins.CurFrameIndex - dieStartFrame) == ConfigMgr.battleCfg.reviveFrameCount)
        {
            return true;
        }
        return false;
    }
}
