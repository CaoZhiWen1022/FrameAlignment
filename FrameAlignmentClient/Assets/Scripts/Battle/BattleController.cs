using Assets.Scripts.FGUI;
using Proto;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 战斗总控制器-创建所有帧同步物体
/// </summary>
public class BattleController : MonoBehaviour
{
    public TextAsset mapData;

    BattlePlayerObject bulePlayer;
    BattlePlayerObject redPlayer;
    FrameSyncMgr frameSyncMgr = new FrameSyncMgr();

    void Start()
    {

        //创建BattlePanel
        GameUIFrame.Instance.uiFrame.OpenByUiid(UIID.BattlePanel);

        PlayerInfo bluePlayerData = new PlayerInfo();
        PlayerInfo redPlayerData = null;

        //初始化角色数据
        for (int i = 0; i < BattleSocket.battlePlayers.Count; i++)
        {
            Proto.PlayerInfo playerInfo = BattleSocket.battlePlayers[i];
            if (playerInfo.Camp == 1)
            {
                bluePlayerData = playerInfo;
            }
            else if (playerInfo.Camp == 2)
            {
                redPlayerData = playerInfo;
            }
        }
        if (redPlayerData == null)
        {
            redPlayerData = new Proto.PlayerInfo();
            redPlayerData.UserId = 9999;
            redPlayerData.Camp = 2;
            redPlayerData.HeroId = 1002;
        }
        GameObject bluePlayerObj = new GameObject("BluePlayer");
        bulePlayer = bluePlayerObj.AddComponent<BattlePlayerObject>();
        bulePlayer.Init(bluePlayerData);

        GameObject redPlayerObj = new GameObject("RedPlayer");
        redPlayer = redPlayerObj.AddComponent<BattlePlayerObject>();
        redPlayer.Init(redPlayerData);
        frameSyncMgr.AddPlayer(bulePlayer.playerLogic);
        frameSyncMgr.AddPlayer(redPlayer.playerLogic);


        //初始化相机
        MobaCamera mobaCamera = Camera.main.AddComponent<MobaCamera>();
        BattlePlayerObject thisPlayer = GetPlayerById(AuthSocket.userInfo.UserId);
        if (thisPlayer.isRed)
        {
            mobaCamera.horizontalOffset = 0;
        }
        mobaCamera.target = thisPlayer.transform;



        //初始化AStar
        AStarManager.Instance.LoadMap(mapData);

        //显示UI
        GameUIFrame.Instance.uiFrame.OpenByUiid(UIID.BattleInitPanel);

        //监听帧数据
        GameEventMgr.Instance.OnWithParam<Proto.FrameDataResponse>(GameEventType._帧数据_参_, FrameDataUpdate);
        GameEventMgr.Instance.OnWithParam<object>(GameEventType._角色发射普工_参_, CreateCommonAtk);
    }

    /// <summary>
    /// 接收到帧数据同步消息
    /// </summary>
    /// <param name="data"></param>
    private void FrameDataUpdate(Proto.FrameDataResponse data)
    {
        frameSyncMgr.FrameSyncDataCpu(data);
    }

    private void CreateCommonAtk(object data)
    {
        if (data is (long playerUid, long targetUid))
        {
            //创建一个普攻子弹
            GameObject obj = new GameObject("CommonAtkBullet");
            var bullet = obj.AddComponent<BulletObject>();
            var logic = bullet.Init(playerUid, targetUid);
            //加入帧同步管理器
            frameSyncMgr.AddBullet(logic);
        }
        else
        {
            Debug.LogError("CreateCommonAtk 参数错误");
        }

    }

    private BattlePlayerObject GetPlayerById(long userId)
    {
        if (bulePlayer != null && bulePlayer.UserId == userId) return bulePlayer;
        if (redPlayer != null && redPlayer.UserId == userId) return redPlayer;
        return null;
    }

    private void OnDestroy()
    {
        GameEventMgr.Instance.OffWithParam<Proto.FrameDataResponse>(GameEventType._帧数据_参_, FrameDataUpdate);
        GameEventMgr.Instance.OffWithParam<object>(GameEventType._角色发射普工_参_, CreateCommonAtk);
    }
}
