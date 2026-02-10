using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// 1.当前状态
/// 2.注册一个状态
/// 3.移除一个状态
/// 4.获取一个状态
/// 5.停止当前状态
/// 6.切换状态的回调
/// 7.切换状态回调委托
/// 8.切换状态
/// 9.判断当前状态是否是某个状态
/// </summary>
public class StateMachine
{
    /// <summary>
    /// 所有的状态集合
    /// </summary>
    private Dictionary<uint, IState> mStateDic = null;
    /// <summary>
    /// 当前状态
    /// </summary>
    private IState mCurrentState = null;

    public IState CurrentState
    {
        get
        {
            return mCurrentState;
        }
    }
    /// <summary>
    /// 当前状态id
    /// </summary>
    public uint CurrentID
    {
        get
        {
            return mCurrentState == null ? 0 : mCurrentState.GetStateID();
        }
    }
    public StateMachine()
    {
        mStateDic = new Dictionary<uint, IState>();
        mCurrentState = null;
    }
    /// <summary>
    /// 注册一个状态
    /// </summary>
    /// <param name="state">状态对象</param>
    /// <returns>成功还是失败</returns>
    public bool RegisterState(IState state)
    {
        if (state == null)
        {
            Debug.LogError("StateMachine.RegisterState state is Null!");
            return false;
        }
        if (mStateDic.ContainsKey(state.GetStateID()))
        {
            Debug.LogError("StateMachine.RegisterState mStateDic hava this key key = " + state.GetStateID());
            return false;
        }
        mStateDic.Add(state.GetStateID(), state);
        return true;
    }
    /// <summary>
    /// 移除一个状态
    /// </summary>
    /// <param name="stateId">状态id</param>
    /// <returns>当状态不存在或者状态正在运行那么返回失败</returns>
    public bool RemoveState(uint stateId)
    {
        if (!mStateDic.ContainsKey(stateId))
        {
            return false;
        }
        if (mCurrentState != null && mCurrentState.GetStateID() == stateId)
        {
            return false;
        }
        mStateDic.Remove(stateId);
        return true;
    }
    /// <summary>
    /// 获取一个状态
    /// </summary>
    /// <param name="stateId">状态ID</param>
    /// <returns></returns>
    public IState GetState(uint stateId)
    {
        IState state = null;
        mStateDic.TryGetValue(stateId, out state);
        return state;
    }
    /// <summary>
    /// 停止当前状态
    /// </summary>
    /// <param name="param1">参数1</param>
    /// <param name="param2">参数2</param>
    public void StopState(object param)
    {
        if (mCurrentState == null)
        {
            return;
        }
        mCurrentState.OnLeave(null, param);
        mCurrentState = null;
    }
    /// <summary>
    /// 切换状态的回调
    /// </summary>
    public BetweenSwitchState BetweenSwitchStateCallBack = null;
    /// <summary>
    /// 切换状态回调委托
    /// </summary>
    /// <param name="from">当前状态</param>
    /// <param name="to">要跳转的状态</param>
    /// <param name="param1">参数1</param>
    /// <param name="param2">参数2</param>
    public delegate void BetweenSwitchState(IState from, IState to, object param1);
    /// <summary>
    /// 切换状态
    /// </summary>
    /// <param name="newStateId">要切换的状态id</param>
    /// <param name="param">参数</param>
    /// <param name="force">是否强制切换，强制切换用于当前状态和目标状态相同，强制时不会执行OnLeave，会执行OnEnter</param>
    /// <returns>如果不存在这个状态或者当前状态等于要切换的那个状态 返回失败</returns>
    public bool SwitchState(uint newStateId, object param, bool force = false)
    {
        if (mCurrentState != null && mCurrentState.GetStateID() == newStateId && force == false)
        {
            return false;
        }
        IState newState = null;
        mStateDic.TryGetValue(newStateId, out newState);
        if (newState == null)
        {
            return false;
        }
        if (mCurrentState != null && !force)
        {
            mCurrentState.OnLeave(newState, param);
        }
        IState oldState = mCurrentState;
        mCurrentState = newState;

        if (BetweenSwitchStateCallBack != null)
        {
            BetweenSwitchStateCallBack(oldState, mCurrentState, param);
        }

        newState.OnEnter(this, oldState, param);

        return true;
    }
    /// <summary>
    /// 判断当前状态是不是某个状态
    /// </summary>
    /// <param name="stateId">状态id</param>
    /// <returns>是？不是</returns>
    public bool IsInState(uint stateId)
    {
        return mCurrentState == null ? false : mCurrentState.GetStateID() == stateId;
    }

    public List<IState> GetAllStates()
    {
        return new List<IState>(mStateDic.Values);
    }

    public void OnUpdate()
    {
        if (mCurrentState != null)
        {
            mCurrentState.OnUpdate();
        }
    }
    public void OnFixedUpdate()
    {
        if (mCurrentState != null)
        {
            mCurrentState.OnFixedUpdate();
        }
    }
    public void OnLeteUpdate()
    {
        if (mCurrentState != null)
        {
            mCurrentState.OnleteUpdate();
        }
    }
}