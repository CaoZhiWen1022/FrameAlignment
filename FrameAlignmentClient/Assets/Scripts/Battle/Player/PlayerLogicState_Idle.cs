using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLogicState_Idle : PlayerLogicState_Base
{
    public PlayerLogicState_Idle(PlayerLogicState state, LogicData logicData, Action<PlayerLogicState, object> action) : base(state, logicData, action)
    {
    }

    public override void OnEnter(StateMachine machine, IState prevState, object param)
    {
        base.OnEnter(machine, prevState, param);
    }

    public override void OnLeave(IState nextState, object param)
    {
        base.OnLeave(nextState, param);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }
}
