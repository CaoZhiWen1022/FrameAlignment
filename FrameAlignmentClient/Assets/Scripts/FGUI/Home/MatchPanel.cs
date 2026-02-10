using Home;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchPanel : FGUIFrame.UIPanel
{
    private UI_MatchPanel ui { get { return m_ui as UI_MatchPanel; } }

    public override void Opened()
    {
        base.Opened();
        GameEventMgr.Instance.On(GameEventType._匹配成功_, MatchSuccess);
        GameEventMgr.Instance.On(GameEventType._取消匹配成功_, CancelMatchSuccess);
        ui.m_cancel.onClick.Add(CancelMatch);
        StartMatch();
    }

    private void StartMatch()
    {
        //发起匹配
        BattleSocket.MatchReq();
    }

    private void MatchSuccess()
    {
        //等待1秒进入battle
        Debug.Log("匹配成功，准备进入战斗");
        ui.m_cancel.touchable = false;
        GameUIFrame.Instance.timerMgr.Once(1, GoToBattleScene, this);

    }

    private void GoToBattleScene()
    {
        SceneMgr.Instance.StartAsyncLoadScene("BattleScene");
    }

    private void CancelMatch()
    {
        BattleSocket.CancelMatchReq();
    }

    private void CancelMatchSuccess()
    {
        CloseThis();
    }

    public override void Closeed()
    {
        base.Closeed();
        GameEventMgr.Instance.Off(GameEventType._匹配成功_, MatchSuccess);
        GameEventMgr.Instance.Off(GameEventType._取消匹配成功_, CancelMatchSuccess);
    }
}
