using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattlePlayerAniName
{
    idle,
    run,
    atk,
    death,
}


/// <summary>
/// 战斗角色动画控制器
/// </summary>
public class BattlePlayerAniController : MonoBehaviour
{
    private Animator animator;
    private BattlePlayerAniName curName = BattlePlayerAniName.idle;

    private void Start()
    {
        animator = transform.GetComponent<Animator>();
    }

    public void SetAni(BattlePlayerAniName type, bool isForce = false)
    {
        if (curName == type && isForce == false) return;
        string aniName = BattlePlayerAniController.GetAniName(type);
        animator.CrossFadeInFixedTime(aniName, 0.2f);
        curName = type;
    }

    public static string GetAniName(BattlePlayerAniName type)
    {
        switch (type)
        {
            case BattlePlayerAniName.run: return "run";
            case BattlePlayerAniName.idle: return "idle1";
            case BattlePlayerAniName.atk: return "attack";
            case BattlePlayerAniName.death: return "death";
        }
        return "idle1";
    }
}
