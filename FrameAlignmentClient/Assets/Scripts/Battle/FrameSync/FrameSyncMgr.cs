using FixedMathSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 帧同步管理器-处理所有帧数据和所有逻辑脚本
/// </summary>
public class FrameSyncMgr
{

    public static FrameSyncMgr ins;

    /// <summary>
    /// 最新收到的帧数据
    /// </summary>
    private Proto.FrameDataResponse curData;
    /// <summary>
    /// 所有帧数据列表
    /// </summary>
    private List<Proto.FrameDataResponse> dataList = new List<Proto.FrameDataResponse>();

    public List<BattlePlayerLogic> battlePlayerLogics = new List<BattlePlayerLogic>();

    public List<BulletLogic> bulletLogics = new List<BulletLogic>();

    public long CurFrameIndex { get { return curData.FrameData.FrameIndex; } }

    public FrameSyncMgr()
    {
        FrameSyncMgr.ins = this;
    }

    /// <summary>
    /// 添加一个玩家
    /// </summary>
    /// <param name="battlePlayerLogic"></param>
    public void AddPlayer(BattlePlayerLogic battlePlayerLogic)
    {
        battlePlayerLogics.Add(battlePlayerLogic);
    }

    public void AddBullet(BulletLogic bulletLogic)
    {
        bulletLogics.Add(bulletLogic);
    }

    public void FrameSyncDataCpu(Proto.FrameDataResponse data)
    {
        var curIndex = data.FrameData.FrameIndex;
        if (curData != null && curIndex != curData.FrameData.FrameIndex + 1)
        {
            Debug.LogError("丢帧！！！！！！！！！！！！");
            return;
        }

        try
        {
            curData = data;
            dataList.Add(data);
            //分发
            foreach (var item in data.FrameData.OpDataList)
            {
                BattlePlayerLogic player = battlePlayerLogics.Find(v => v.uid == item.UserId);
                player.SetFrameData(item);
            }
            //更新所有玩家逻辑
            foreach (var item in battlePlayerLogics)
            {
                item.Update();
            }
            //更新所有子弹逻辑
            foreach (var item in bulletLogics)
            {
                item.OnUpdate();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"逻辑帧更新异常：{ex.Message}");
        }
    }


    public void OnBulletHit(long uid, Fixed64 hitValue)
    {
        var logic = battlePlayerLogics.Find(v => v.uid == uid);
        logic.TakeHarm(hitValue);
    }


    public Vector3d GetPlayerPosByUid(long uid)
    {
        var player = battlePlayerLogics.Find(v => v.playerInfo.UserId == uid);
        var pos = player != null ? player.PosV3 : Vector3d.Zero;
        //创建一个新对象，避免污染逻辑层数据
        return new Vector3d(pos.x, pos.y, pos.z);
    }

    public Fixed64 GetPlayerRotateByUid(long uid)
    {
        var player = battlePlayerLogics.Find(v => v.playerInfo.UserId == uid);
        var rotate = player != null ? player.Rotate : Vector3d.Zero;
        //创建一个新对象，避免污染逻辑层数据
        return rotate.y;
    }

    public int GetPlayerHeroIdByUid(long uid)
    {
        var player = battlePlayerLogics.Find(v => v.playerInfo.UserId == uid);
        return player != null ? player.playerInfo.HeroId : 1001;
    }
}
