using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroSocket
{
    public static Proto.HeroInfo heroInfo;

    public static void GetHeroInfoReq()
    {
        Proto.GetHeroInfoRequest request = new Proto.GetHeroInfoRequest();
        request.Base = new Proto.BaseRequest();
        request.Base.Id = Proto.ApiId.ApiGetHeroInfoReq;
        GameSocketMgr.Instance.Send(request);
    }

    public static void GetHeroInfoResp(byte[] data)
    {
        Proto.GetHeroInfoResponse message = Proto.GetHeroInfoResponse.Parser.ParseFrom(data);
        heroInfo = message.HeroInfo;
        Debug.Log("接收到英雄信息回调");
        GameEventMgr.Instance.Emit(GameEventType._获取英雄信息成功_);
    }

    public static void SetUseHeroReq(int heroId)
    {
        Proto.SetUseHeroRequest request = new Proto.SetUseHeroRequest();
        request.Base = new Proto.BaseRequest();
        request.Base.Id = Proto.ApiId.ApiSetUseHeroReq;
        request.HeroId = heroId;
        GameSocketMgr.Instance.Send(request);
    }

    public static void SetUseHeroResp(byte[] data)
    {
        Proto.SetUseHeroResponse message = Proto.SetUseHeroResponse.Parser.ParseFrom(data);
        Debug.Log("修改英雄信息回调");
        heroInfo = message.HeroInfo;
        GameEventMgr.Instance.Emit(GameEventType._修改英雄信息成功_);
    }
}
