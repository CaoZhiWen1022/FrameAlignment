using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocketHandler
{
    private static SocketHandler instance;
    public static SocketHandler Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new SocketHandler();
            }
            return instance;
        }
    }

    public Dictionary<Proto.ApiId, Action<byte[]>> respHandler;

    private SocketHandler()
    {
        respHandler = new Dictionary<Proto.ApiId, Action<byte[]>>();
        register(Proto.ApiId.ApiRegisterResp, AuthSocket.RegisterResp);
        register(Proto.ApiId.ApiLoginResp, AuthSocket.LoginResp);
        register(Proto.ApiId.ApiGetHeroInfoResp, HeroSocket.GetHeroInfoResp);
        register(Proto.ApiId.ApiSetUseHeroResp, HeroSocket.SetUseHeroResp);
        register(Proto.ApiId.ApiMatchResp, BattleSocket.MatchResp);
        register(Proto.ApiId.ApiMatchSuccessResp, BattleSocket.MatchSuccessResp);
        register(Proto.ApiId.ApiCancelMatchResp, BattleSocket.CancelMatchResp);
        register(Proto.ApiId.ApiBattleReadyResp, BattleSocket.BattleReadyResp);
        register(Proto.ApiId.ApiBattleCountdownResp, BattleSocket.BattleCountdownResp);
        register(Proto.ApiId.ApiBattleStartResp, BattleSocket.BattleStartResp);
        register(Proto.ApiId.ApiFrameDataResp, BattleSocket.BattleFrameDataResp);
        register(Proto.ApiId.ApiFrameSyncResp, BattleSocket.FrameSyncResp);

    }

    private void register(Proto.ApiId apiId, Action<byte[]> action)
    {
        //·ÀÖ¹ÖØ¸´×¢²á
        if (respHandler.ContainsKey(apiId))
        {
            Debug.LogWarning(apiId + "ÖØ¸´×¢²á£¬ÒÑÌø¹ý");
            return;
        }
        respHandler[apiId] = action;
    }

    public Action<byte[]> getHandler(Proto.ApiId apiId)
    {
        if (respHandler.ContainsKey(apiId))
        {
            return respHandler[apiId];
        }
        else
        {
            return null;
        }

    }
}
