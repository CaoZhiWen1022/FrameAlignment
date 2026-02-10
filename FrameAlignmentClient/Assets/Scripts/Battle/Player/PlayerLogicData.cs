using FixedMathSharp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家逻辑数据
/// </summary>
public class LogicData
{
    //初始位置
    public Vector3d initPos;
    //初始旋转
    public Vector3d initRot;

    // 逻辑位置
    public Vector3d posV3;
    // 逻辑旋转
    public Vector3d rotate;
    // 玩家数据 
    public BattlePlayerData playerData;
    
}
