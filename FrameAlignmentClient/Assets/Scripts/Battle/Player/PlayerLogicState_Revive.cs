using FixedMathSharp;
using Proto;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerLogicState_Revive : PlayerLogicState_Base
{
    private long startFrameIndex;


    public PlayerLogicState_Revive(PlayerLogicState state, LogicData logicData, Action<PlayerLogicState, object> action) : base(state, logicData, action)
    {
    }

    public override void OnEnter(StateMachine machine, IState prevState, object param)
    {
        startFrameIndex = FrameSyncMgr.ins.CurFrameIndex;
        logicData.posV3 = new Vector3d(logicData.initPos.x, logicData.initPos.y, logicData.initPos.z);
        logicData.rotate = new Vector3d(logicData.initRot.x, logicData.initRot.y, logicData.initRot.z);
        logicData.playerData.hp = logicData.playerData.maxHp;
    }

    public override void OnLeave(IState nextState, object param)
    {

    }

    public override void OnUpdate()
    {
        //检测是否该复活了
        if ((Fixed64)(FrameSyncMgr.ins.CurFrameIndex - startFrameIndex) == (Fixed64)15.0)
        {
            //进入idle状态
            SwitchState((uint)PlayerLogicState.idle);
        }
    }


    public override bool IsAllowSwitch(uint target)
    {
        if (target == (uint)PlayerLogicState.idle && (Fixed64)(FrameSyncMgr.ins.CurFrameIndex - startFrameIndex) == (Fixed64)15.0)
        {
            return true;
        }
        return false;
    }
}
