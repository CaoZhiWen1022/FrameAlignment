using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// 全局事件管理器（单例模式）
/// 基础方法（On/Off/Emit/OffAll）：无参事件（大部分场景使用）
/// 带参方法（OnWithParam/OffWithParam/EmitWithParam/OffAllWithParam）：带泛型参数事件
/// 事件标识：使用GameEventType枚举（已提前定义）
/// </summary>
public class GameEventMgr : MonoBehaviour
{
    #region 单例模式（保持不变，全局唯一）
    private static GameEventMgr _instance;
    public static GameEventMgr Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("[GameEventMgr]");
                _instance = go.AddComponent<GameEventMgr>();
                DontDestroyOnLoad(go); // 场景切换不销毁
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    #region 事件存储（替换为GameEventType枚举作为Key，分离无参/带参）
    // 1. 无参事件字典：key=GameEventType枚举，value=无参委托（Action）
    private readonly Dictionary<GameEventType, Action> _eventDictNoParam = new Dictionary<GameEventType, Action>();

    // 2. 带参事件字典：key=GameEventType枚举，value=泛型委托（Delegate）
    private readonly Dictionary<GameEventType, Delegate> _eventDictWithParam = new Dictionary<GameEventType, Delegate>();
    #endregion

    #region 基础无参事件（核心，大部分场景使用，枚举作为事件标识）
    /// <summary>
    /// 订阅【无参】事件（基础方法，默认常用）
    /// </summary>
    /// <param name="eventType">事件类型（GameEventType枚举）</param>
    /// <param name="callback">无参回调方法</param>
    public void On(GameEventType eventType, Action callback)
    {
        if (callback == null)
        {
            Debug.LogError("无参事件回调不能为null！");
            return;
        }

        lock (_eventDictNoParam)
        {
            // 事件不存在则初始化，存在则合并委托（支持多回调订阅）
            if (!_eventDictNoParam.ContainsKey(eventType))
            {
                _eventDictNoParam[eventType] = null;
            }
            _eventDictNoParam[eventType] += callback;
        }
    }

    /// <summary>
    /// 派发【无参】事件（基础方法，默认常用）
    /// </summary>
    /// <param name="eventType">事件类型（GameEventType枚举）</param>
    public void Emit(GameEventType eventType)
    {
        lock (_eventDictNoParam)
        {
            // 尝试获取事件，避免直接访问抛出异常
            if (_eventDictNoParam.TryGetValue(eventType, out Action callback))
            {
                try
                {
                    callback?.Invoke(); // 安全调用，避免回调为null
                }
                catch (Exception ex)
                {
                    Debug.LogError($"无参事件「{eventType}」派发异常：{ex.Message}");
                }
            }
        }
    }

    /// <summary>
    /// 取消订阅【无参】事件（基础方法，默认常用）
    /// </summary>
    /// <param name="eventType">事件类型（GameEventType枚举）</param>
    /// <param name="callback">要取消的无参回调（必须与订阅时引用一致）</param>
    public void Off(GameEventType eventType, Action callback)
    {
        if (callback == null)
        {
            return;
        }

        lock (_eventDictNoParam)
        {
            if (_eventDictNoParam.ContainsKey(eventType))
            {
                // 移除指定回调
                _eventDictNoParam[eventType] -= callback;

                // 回调为空时移除事件，释放内存
                if (_eventDictNoParam[eventType] == null)
                {
                    _eventDictNoParam.Remove(eventType);
                }
            }
        }
    }
    #endregion

    #region 带参事件（后缀WithParam，明确区分，按需使用）
    /// <summary>
    /// 订阅【带参】事件（按需使用，泛型支持任意参数类型）
    /// </summary>
    /// <typeparam name="T">参数类型（int/string/自定义类等）</typeparam>
    /// <param name="eventType">事件类型（GameEventType枚举）</param>
    /// <param name="callback">带参回调方法</param>
    public void OnWithParam<T>(GameEventType eventType, Action<T> callback)
    {
        if (callback == null)
        {
            Debug.LogError("带参事件回调不能为null！");
            return;
        }

        lock (_eventDictWithParam)
        {
            if (!_eventDictWithParam.ContainsKey(eventType))
            {
                _eventDictWithParam[eventType] = null;
            }
            // 合并泛型委托
            _eventDictWithParam[eventType] = (Action<T>)_eventDictWithParam[eventType] + callback;
        }
    }

    /// <summary>
    /// 派发【带参】事件（按需使用，泛型参数需与订阅时一致）
    /// </summary>
    /// <typeparam name="T">参数类型</typeparam>
    /// <param name="eventType">事件类型（GameEventType枚举）</param>
    /// <param name="param">事件参数</param>
    public void EmitWithParam<T>(GameEventType eventType, T param)
    {
        lock (_eventDictWithParam)
        {
            if (_eventDictWithParam.TryGetValue(eventType, out Delegate del))
            {
                // 转换为对应泛型委托，确保类型匹配
                Action<T> callback = del as Action<T>;
                if (callback != null)
                {
                    try
                    {
                        callback.Invoke(param);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"带参事件「{eventType}」派发异常：{ex.Message}");
                    }
                }
                else
                {
                    Debug.LogWarning($"带参事件「{eventType}」委托类型不匹配！");
                }
            }
        }
    }

    /// <summary>
    /// 取消订阅【带参】事件（按需使用，回调引用需与订阅时一致）
    /// </summary>
    /// <typeparam name="T">参数类型</typeparam>
    /// <param name="eventType">事件类型（GameEventType枚举）</param>
    /// <param name="callback">要取消的带参回调</param>
    public void OffWithParam<T>(GameEventType eventType, Action<T> callback)
    {
        if (callback == null)
        {
            return;
        }

        lock (_eventDictWithParam)
        {
            if (_eventDictWithParam.ContainsKey(eventType))
            {
                // 移除指定泛型回调
                _eventDictWithParam[eventType] = (Action<T>)_eventDictWithParam[eventType] - callback;

                // 委托为空时移除事件，释放内存
                if (_eventDictWithParam[eventType] == null)
                {
                    _eventDictWithParam.Remove(eventType);
                }
            }
        }
    }
    #endregion

    #region 全局清空事件（支持无参/带参，统一管理）
    /// <summary>
    /// 清空【单个无参事件】或【所有无参事件】
    /// </summary>
    /// <param name="eventType">可选：指定无参事件类型，为null则清空所有无参事件（枚举可传默认值替代）</param>
    public void OffAll(GameEventType? eventType = null)
    {
        lock (_eventDictNoParam)
        {
            if (eventType.HasValue)
            {
                if (_eventDictNoParam.ContainsKey(eventType.Value))
                {
                    _eventDictNoParam.Remove(eventType.Value);
                    Debug.Log($"无参事件「{eventType.Value}」已清空");
                }
            }
            else
            {
                _eventDictNoParam.Clear();
                Debug.Log("所有【无参事件】已清空");
            }
        }
    }

    /// <summary>
    /// 清空【单个带参事件】或【所有带参事件】
    /// </summary>
    /// <param name="eventType">可选：指定带参事件类型，为null则清空所有带参事件</param>
    public void OffAllWithParam(GameEventType? eventType = null)
    {
        lock (_eventDictWithParam)
        {
            if (eventType.HasValue)
            {
                if (_eventDictWithParam.ContainsKey(eventType.Value))
                {
                    _eventDictWithParam.Remove(eventType.Value);
                    Debug.Log($"带参事件「{eventType.Value}」已清空");
                }
            }
            else
            {
                _eventDictWithParam.Clear();
                Debug.Log("所有【带参事件】已清空");
            }
        }
    }

    /// <summary>
    /// 清空【所有事件】（无参+带参，全局清理）
    /// </summary>
    public void OffAllGlobal()
    {
        OffAll();
        OffAllWithParam();
        Debug.Log("所有事件（无参+带参）已全部清空");
    }
    #endregion

    #region 场景销毁时清理（避免内存泄漏）
    private void OnDestroy()
    {
        if (_instance == this)
        {
            OffAllGlobal(); // 清空所有事件
            _instance = null;
        }
    }
    #endregion
}