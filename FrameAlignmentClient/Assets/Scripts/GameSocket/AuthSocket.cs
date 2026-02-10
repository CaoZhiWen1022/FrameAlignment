using Proto;
using System.Collections;
using UnityEngine;

public class AuthSocket
{

    public static Proto.UserInfo userInfo;
    public static void RegisterReq(string account, string possword, string nickName)
    {
        Proto.RegisterRequest request = new Proto.RegisterRequest();
        request.Base = new Proto.BaseRequest();
        request.Base.Id = Proto.ApiId.ApiRegisterReq;
        request.Account = account;
        request.Password = possword;
        request.Nickname = nickName;
        GameSocketMgr.Instance.Send(request);
    }

    public static void RegisterResp(byte[] data)
    {
        Proto.RegisterResponse message = Proto.RegisterResponse.Parser.ParseFrom(data);
        Debug.Log("接收到注册回调");
        GameEventMgr.Instance.Emit(GameEventType._注册成功_);
    }

    public static void LoginReq(string account, string password)
    {
        Proto.LoginRequest request = new Proto.LoginRequest();
        request.Base = new Proto.BaseRequest();
        request.Base.Id = Proto.ApiId.ApiLoginReq;
        request.Account = account;
        request.Password = password;
        GameSocketMgr.Instance.Send(request);
    }

    public static void LoginResp(byte[] data)
    {
        Debug.Log("登录成功");
        Proto.LoginResponse message = Proto.LoginResponse.Parser.ParseFrom(data);
        userInfo = message.UserInfo;
        GameEventMgr.Instance.Emit(GameEventType._登录成功_);
    }
}