using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroConfigMgr
{
    private List<HeroCfg> HeroList;

    public HeroConfigMgr()
    {
        HeroList = new List<HeroCfg>()
        {
            new HeroCfg(id:1001,
                name:"瑞天帝",
                prefabName:"Ryze",
                hp:1000,
                atk:20,
                speed:5,
                commonAtkType:CommonAtkType._远程,
                commonAtkDistance:7,
                atkFrameCount:15,
                atkHarmFrame:2,
                commonAtkPrefab:"RyzeBullet"
            ),
            new HeroCfg(id:1002,
                name:"剑圣",
                prefabName:"YiDaShi",
                hp:1200,
                atk:30,
                speed:8,
                commonAtkType:CommonAtkType._近战,
                atkFrameCount:15,
                atkHarmFrame:5,
                commonAtkDistance:5
            ),
        };
    }

    public HeroCfg GetHeroById(int id)
    {
        return this.HeroList.Find(hero => hero.id == id);
    }

    public List<HeroCfg> GetAllHero()
    {
        return this.HeroList;
    }
}

public class HeroCfg
{
    public int id;
    public string name;
    public string prefabName;
    public int hp;
    public int atk;
    public int speed;
    public CommonAtkType commonAtkType;
    public string commonAtkPrefab;
    public int commonAtkDistance;
    public int atkFrameCount;
    public int atkHarmFrame;
    public HeroCfg(int id, string name, string prefabName, int hp, int atk, int speed, CommonAtkType commonAtkType, int commonAtkDistance, int atkFrameCount, int atkHarmFrame, string commonAtkPrefab = "")
    {
        this.id = id;
        this.name = name;
        this.prefabName = prefabName;
        this.hp = hp;
        this.atk = atk;
        this.speed = speed;
        this.commonAtkType = commonAtkType;
        this.commonAtkPrefab = commonAtkPrefab;
        this.commonAtkDistance = commonAtkDistance;
        this.atkFrameCount = atkFrameCount;
        this.atkHarmFrame = atkHarmFrame;
    }
}

public enum CommonAtkType
{
    _近战,
    _远程,
}