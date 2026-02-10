using UnityEngine;
using FGUIFrame;

/// <summary>
/// 游戏框架主类
/// </summary>
public class GameUIFrame : MonoBehaviour
{
    private static GameUIFrame _instance;
    public static GameUIFrame Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject("GameUIFrame");
                _instance = go.AddComponent<GameUIFrame>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    public UIFrame uiFrame;
    public TimerMgr timerMgr;

    /// <summary>
    /// 框架初始化
    /// </summary>
    public void Init()
    {
        Debug.Log("GameFrame Init");
        this.uiFrame = new UIFrame();
        this.uiFrame.GameLaunchInit();
        Debug.Log("uiFrame Init");

        var timerMgrGO = new GameObject("TimerMgr");
        timerMgrGO.transform.SetParent(this.transform);
        this.timerMgr = timerMgrGO.AddComponent<TimerMgr>();
        Debug.Log("timerMgr Init");
    }

    /// <summary>
    /// 初始化UI框架配置
    /// </summary>
    public void InitUIFrameConfig(int frameWidth, int frameHeight,
        System.Collections.Generic.List<string> initLoadPkgs,
        System.Collections.Generic.List<string> permanentPkgs,
        int maxPkgs, float popupMaskAlpha)
    {
        UIFrameConfig.Init(frameWidth, frameHeight, initLoadPkgs, permanentPkgs, maxPkgs, popupMaskAlpha);
    }
}

