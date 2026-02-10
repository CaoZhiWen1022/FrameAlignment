using FixedMathSharp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePlayerData
{
    public int heroId;

    /// <summary>
    /// 最大血量
    /// </summary>
    public Fixed64 maxHp;
    /// <summary>
    /// 血量
    /// </summary>
    public Fixed64 hp;
    /// <summary>
    /// 攻击力
    /// </summary>
    public Fixed64 atk;
    /// <summary>
    /// 移动速度
    /// </summary>
    public Fixed64 speed;
    /// <summary>
    /// 普工距离
    /// </summary>
    public Fixed64 commonAtkDistance;
    /// <summary>
    /// 普工总帧数
    /// </summary>
    public Fixed64 atkFrameCount;
    /// <summary>
    /// 普工伤害触发帧数-远程释放子弹-近战直接结算伤害
    /// </summary>
    public Fixed64 atkHarmFrame;
    public void InitData(int heroId)
    {
        this.heroId = heroId;
        HeroCfg cfg = ConfigMgr.heroCfg.GetHeroById(heroId);
        hp = (Fixed64)cfg.hp;
        maxHp = hp;
        atk = (Fixed64)cfg.atk;
        speed = (Fixed64)cfg.speed;
        commonAtkDistance = (Fixed64)cfg.commonAtkDistance;
        atkFrameCount = (Fixed64)cfg.atkFrameCount;
        atkHarmFrame = (Fixed64)cfg.atkHarmFrame;
    }
}
