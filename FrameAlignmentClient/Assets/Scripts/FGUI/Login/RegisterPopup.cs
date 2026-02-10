using Login;
using System.Collections;
using UnityEngine;
public class RegisterPopup : FGUIFrame.UIPopup
{

    private UI_RegisterPopup _ui { get { return base.m_ui as UI_RegisterPopup; } }

    public override void Opened()
    {
        base.Opened();
        _ui.m_register.onClick.Add(OnRegisterClick);

        GameEventMgr.Instance.On(GameEventType._注册成功_, OnRegisterSuccess);
    }

    private void OnRegisterClick()
    {
        string account = _ui.m_account.m_input.text;
        string password = _ui.m_password.m_input.text;
        string userName = _ui.m_username.m_input.text;
        //请求注册
        AuthSocket.RegisterReq(account, password, userName);
    }

    private void OnRegisterSuccess()
    {
        this.CloseThis();
    }

    public override void Closeed()
    {
        base.Closeed();
        GameEventMgr.Instance.Off(GameEventType._注册成功_, OnRegisterSuccess);
    }
}