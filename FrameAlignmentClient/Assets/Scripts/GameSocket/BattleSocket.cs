using FixedMathSharp;
using Proto;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSocket
{

    public static List<PlayerInfo> battlePlayers;

    /// <summary>
    /// 最新收到的帧数据
    /// </summary>
    private static FrameDataResponse newFrameData;

    public static long CurFrameIndex
    {
        get { return newFrameData.FrameData.FrameIndex; }
    }

    public static void MatchReq()
    {
        Proto.MatchRequest request = new Proto.MatchRequest();
        request.Base = new Proto.BaseRequest();
        request.Base.Id = Proto.ApiId.ApiMatchReq;
        GameSocketMgr.Instance.Send(request);
    }

    public static void MatchResp(byte[] data)
    {
        Debug.Log("MatchResp");
    }

    public static void MatchSuccessResp(byte[] data)
    {
        Proto.MatchSuccessResponse matchSuccess = Proto.MatchSuccessResponse.Parser.ParseFrom(data);
        battlePlayers = new List<PlayerInfo>(matchSuccess.Players);
        Debug.Log("匹配成功");
        GameEventMgr.Instance.Emit(GameEventType._匹配成功_);
    }

    public static void CancelMatchReq()
    {
        Proto.CancelMatchRequest request = new Proto.CancelMatchRequest();
        request.Base = new Proto.BaseRequest();
        request.Base.Id = Proto.ApiId.ApiCancelMatchReq;
        GameSocketMgr.Instance.Send(request);
    }

    public static void CancelMatchResp(byte[] data)
    {
        Proto.CancelMatchResponse matchSuccess = Proto.CancelMatchResponse.Parser.ParseFrom(data);
        Debug.Log("取消匹配成功");
        GameEventMgr.Instance.Emit(GameEventType._取消匹配成功_);
    }

    public static void BattleReadyReq()
    {
        Proto.BattleReadyRequest request = new Proto.BattleReadyRequest();
        request.Base = new Proto.BaseRequest();
        request.Base.Id = Proto.ApiId.ApiBattleReadyReq;
        GameSocketMgr.Instance.Send(request);
    }

    public static void BattleReadyResp(byte[] data)
    {
        //Debug.Log("准备完成回调，无作用不处理");
    }

    public static void BattleCountdownResp(byte[] data)
    {
        Proto.BattleCountdownResponse message = Proto.BattleCountdownResponse.Parser.ParseFrom(data);
        GameEventMgr.Instance.EmitWithParam<int>(GameEventType._战斗倒计时_参_, message.Countdown);
    }

    public static void BattleStartResp(byte[] data)
    {
        GameEventMgr.Instance.Emit(GameEventType._战斗开始_);
    }

    public static void FrameSyncRequest(Proto.OpData opData)
    {
        Proto.FrameSyncRequest request = new FrameSyncRequest();
        request.Base = new Proto.BaseRequest();
        request.Base.Id = ApiId.ApiFrameSyncReq;
        request.OpData = opData;
        request.FrameIndex = newFrameData.FrameData.FrameIndex;
        GameSocketMgr.Instance.Send(request);
    }

    public static void FrameSyncResp(byte[] data)
    {

    }

    public static void BattleFrameDataResp(byte[] data)
    {
        FrameDataResponse frameData = FrameDataResponse.Parser.ParseFrom(data);
        newFrameData = frameData;
        GameEventMgr.Instance.EmitWithParam<FrameDataResponse>(GameEventType._帧数据_参_, frameData);
    }
}
