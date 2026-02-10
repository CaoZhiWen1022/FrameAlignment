using FixedMathSharp;
using Proto;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 战场角色控制器
/// </summary>
public class BattlePlayerObject : MonoBehaviour
{
    /// <summary>
    /// 角色逻辑脚本
    /// </summary>
    public BattlePlayerLogic playerLogic;
    /// <summary>
    /// 角色渲染脚本
    /// </summary>
    public BattlePlayerView playerView;

    /// <summary>
    /// 玩家数据
    /// </summary>
    public PlayerInfo playerInfo;

    /// <summary>
    /// 英雄实例
    /// </summary>
    GameObject heroObj = null;

    public long UserId { get { return playerInfo.UserId; } }

    public bool isRed { get { return playerInfo.Camp == 2; } }

    // Start is called before the first frame update
    void Start()
    {

    }

    public void Init(PlayerInfo playerData)
    {
        this.playerInfo = playerData;

        //初始化英雄
        HeroCfg cfg = ConfigMgr.heroCfg.GetHeroById(playerInfo.HeroId);
        Object prefab = Resources.Load("HeroModel/" + cfg.prefabName + "/" + cfg.prefabName);
        heroObj = Instantiate(prefab) as GameObject;
        heroObj.transform.SetParent(this.transform);
        heroObj.transform.localPosition = Vector3.zero;
        heroObj.name = "Player";
        //禁用自身碰撞器
        if (playerInfo.UserId == AuthSocket.userInfo.UserId)
        {
            heroObj.GetComponent<BoxCollider>().enabled = false;
        }

        //初始化逻辑层和渲染层
        playerLogic = new BattlePlayerLogic();
        if (playerInfo.Camp == 1)
        {
            playerLogic.Init(ConfigMgr.battleCfg.blueInitV3, ConfigMgr.battleCfg.blueInitRotate, playerData);
        }
        else
        {
            Vector3d redInitV3 = playerInfo.UserId == 9999 ? ConfigMgr.battleCfg.dummyInitV3 : ConfigMgr.battleCfg.redInitV3;
            Vector3d redInitRotate = playerInfo.UserId == 9999 ? ConfigMgr.battleCfg.dummyInitRotate : ConfigMgr.battleCfg.redInitRotate;
            playerLogic.Init(redInitV3, redInitRotate, playerData);
        }

        playerView = transform.AddComponent<BattlePlayerView>();
        playerView.Init(playerLogic);

        //如果该玩家是自己，则添加控制脚本
        if (playerInfo.UserId == AuthSocket.userInfo.UserId)
        {
            this.AddComponent<BattlePlayerController>();
        }

        //初始化光圈
        string guangquanName = playerInfo.UserId == AuthSocket.userInfo.UserId ? ConfigMgr.battleCfg.blue_guangquan : ConfigMgr.battleCfg.red_guangquan;
        GameObject guangquanPrefab = Resources.Load<GameObject>("Effect/" + guangquanName);
        GameObject guangquanObj = Instantiate(guangquanPrefab);
        guangquanObj.transform.SetParent(this.transform);
        guangquanObj.transform.localPosition = Vector3.zero;
        guangquanObj.transform.localPosition = new Vector3(0, 0.1f, 0);
    }

    public void FrameDataUpdate(Proto.OpData frameData)
    {
        playerLogic.SetFrameData(frameData);
    }
}
