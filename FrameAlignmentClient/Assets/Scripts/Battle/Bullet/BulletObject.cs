using FixedMathSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BulletObject : MonoBehaviour
{
    /// <summary>
    /// 初始化子弹对象
    /// </summary>
    /// <param name="playerUid">所属玩家Uid</param>
    /// <param name="targetUid">攻击目标Uid</param>
    public BulletLogic Init(long playerUid, long targetUid)
    {
        //初始化逻辑位置
        var logicPos = GetBulletObjInitPos(playerUid);
        //创建子弹实例
        int heroId = FrameSyncMgr.ins.GetPlayerHeroIdByUid(playerUid);
        HeroCfg cfg = ConfigMgr.heroCfg.GetHeroById(heroId);
        UnityEngine.Object bulletPrefab = Resources.Load("Effect/" + cfg.commonAtkPrefab);
        GameObject bulletObj = GameObject.Instantiate(bulletPrefab) as GameObject;
        bulletObj.transform.SetParent(transform);
        bulletObj.transform.localPosition = Vector3.zero;
        //创建逻辑脚本
        BulletLogic bulletLogic = new BulletLogic(logicPos, Vector3d.Zero, targetUid);
        //创建视图脚本
        BulletView bulletView = gameObject.AddComponent<BulletView>();
        bulletView.Init(bulletLogic);
        return bulletLogic;
    }

    private Vector3d GetBulletObjInitPos(long playerUid)
    {
        var rotate = FrameSyncMgr.ins.GetPlayerRotateByUid(playerUid);
        var playerPos = FrameSyncMgr.ins.GetPlayerPosByUid(playerUid);

        // 定义子弹相对于角色中心的偏移量（定点数）
        // 假设：向前方偏移 0.8个单位，向上方（高度）偏移 1.2个单位
        Fixed64 offsetZ = (Fixed64)0.2;
        Fixed64 offsetY = (Fixed64)2;
        Fixed64 offsetX = Fixed64.Zero;

        // 将逻辑朝向（角度）转为弧度
        // 注意：Unity的方向 0度是正Z，顺时针增加。Math.Atan2和Sin/Cos的标准坐标系可能需要换算
        // 简单的做法是利用角度转向量

        double angleRad = (double)rotate * (Math.PI / 180.0);

        // 计算世界空间偏移
        // 在Unity坐标系中：
        // x' = x*cos(y) + z*sin(y)
        // z' = -x*sin(y) + z*cos(y)
        Fixed64 worldOffsetX = (Fixed64)(Math.Sin(angleRad) * (double)offsetZ);
        Fixed64 worldOffsetZ = (Fixed64)(Math.Cos(angleRad) * (double)offsetZ);

        return new Vector3d(
            playerPos.x + worldOffsetX,
            playerPos.y + offsetY,
            playerPos.z + worldOffsetZ
        );
    }
}
