using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家逻辑状态机
/// </summary>
public class PlayerLogicStateMachine
{
    StateMachine stateMachine;

    LogicData logicData;

    /// <summary>
    /// 状态切换回调
    /// </summary>
    public Action<PlayerLogicState> StateSwitchHandler;

    public PlayerLogicStateMachine(LogicData logicData, long uid)
    {
        stateMachine = new StateMachine();
        this.logicData = logicData;
        stateMachine.RegisterState(new PlayerLogicState_Move(PlayerLogicState.move, logicData, SwitchState));
        stateMachine.RegisterState(new PlayerLogicState_Idle(PlayerLogicState.idle, logicData, SwitchState));
        stateMachine.RegisterState(new PlayerLogicState_Atk(PlayerLogicState.atk, logicData, SwitchState));
        stateMachine.RegisterState(new PlayerLogicState_AtkMove(PlayerLogicState.atkMove, logicData, SwitchState));
        stateMachine.RegisterState(new PlayerLogicState_Die(PlayerLogicState.die, logicData, SwitchState));
        stateMachine.RegisterState(new PlayerLogicState_Revive(PlayerLogicState.revive, logicData, SwitchState));

        foreach (var state in stateMachine.GetAllStates())
        {
            if (state is PlayerLogicState_Base playerStateBase)
            {
                playerStateBase.SetUid(uid);
            }
        }

    }

    public void SwitchState(PlayerLogicState state, object param)
    {

        if (stateMachine.CurrentState == null || stateMachine.CurrentState.IsAllowSwitch((uint)state))
        {
            uint targetState = (uint)state;
            uint curState = stateMachine.CurrentID;
            stateMachine.SwitchState((uint)state, param, targetState == curState);
            StateSwitchHandler?.Invoke(state);
        }
        else
        {
            //当前不允许切换状态
            Debug.LogWarning("当前不允许切换状态");
        }
    }

    public void Update()
    {
        stateMachine.OnUpdate();
    }
}

public enum PlayerLogicState : uint
{
    idle,
    move,
    atkMove,
    atk,
    die,
    revive,
}
