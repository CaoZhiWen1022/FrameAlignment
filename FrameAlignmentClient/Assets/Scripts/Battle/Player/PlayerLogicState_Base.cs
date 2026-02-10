using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLogicState_Base : IState
{


    PlayerLogicState state;
    protected LogicData logicData;
    private Action<PlayerLogicState, object> SwitchStateAction;
    public long uid;
    public uint GetStateID()
    {
        return (uint)state;
    }

    public PlayerLogicState_Base(PlayerLogicState state, LogicData logicData, Action<PlayerLogicState, object> action)
    {
        this.state = state;
        this.logicData = logicData;
        SwitchStateAction = action;
    }

    public void SetUid(long uid)
    {
        this.uid = uid;
    }

    public virtual void OnEnter(StateMachine machine, IState prevState, object param)
    {
    }

    public virtual void OnFixedUpdate()
    {

    }

    public virtual void OnLeave(IState nextState, object param)
    {
    }

    public virtual void OnleteUpdate()
    {
    }

    public virtual void OnUpdate()
    {
    }

    public virtual bool IsAllowSwitch(uint target)
    {
        return true;
    }

    public void SwitchState(PlayerLogicState state, object param = null)
    {
        SwitchStateAction?.Invoke(state, param);
    }
}
