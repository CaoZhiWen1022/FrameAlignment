using Assets.Scripts.FGUI;
using Login;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginPanel : FGUIFrame.UIPanel
{
    private UI_LoginPanel _ui { get { return base.m_ui as UI_LoginPanel; } }

    public override void Opened()
    {
        base.Opened();
        Debug.Log("LoginPanel Opened");

        _ui.m_loginBtn.onClick.Add(OnLoginBtnClick);
        _ui.m_registerBtn.onClick.Add(OnRegisterBtnClick);

        GameEventMgr.Instance.On(GameEventType._登录成功_, OnLoginSuccess);
        GameEventMgr.Instance.On(GameEventType._获取英雄信息成功_, GetDataResp);
    }

    private void OnLoginBtnClick()
    {
        string account = _ui.m_account.m_input.text;
        string password = _ui.m_possword.m_input.text;
        AuthSocket.LoginReq(account, password);
    }

    private void OnRegisterBtnClick()
    {
        GameUIFrame.Instance.uiFrame.popupQueueMgr.Push(new FGUIFrame.OpenUIParam((int)Assets.Scripts.FGUI.UIID.RegisterPopup));
    }

    private void OnLoginSuccess()
    {
        Debug.Log("登录成功，开始获取必要数据");
        HeroSocket.GetHeroInfoReq();
    }

    private void GetDataResp()
    {
        //检查所有所需数据
        if (HeroSocket.heroInfo == null) return;

        Debug.Log("必要数据获取完成，进入主场景");
        GoToHomeScene();
    }

    private void GoToHomeScene()
    {
        SceneMgr.Instance.StartAsyncLoadScene("HomeScene");
    }

    public override void Closeed()
    {
        base.Closeed();
        GameEventMgr.Instance.Off(GameEventType._登录成功_, OnLoginSuccess);
        GameEventMgr.Instance.Off(GameEventType._获取英雄信息成功_, GetDataResp);
    }
}

