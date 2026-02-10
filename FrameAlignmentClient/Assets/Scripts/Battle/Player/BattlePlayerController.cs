using FixedMathSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家控制脚本
/// </summary>
public class BattlePlayerController : MonoBehaviour
{
    Camera gameCamera;
    // Start is called before the first frame update
    void Start()
    {
        gameCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1) && gameCamera != null)
        {
            Ray ray = gameCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.name == "ground")
                {
                    Debug.Log("点击地面");
                    //地面移动
                    Fixed64 fixX = (Fixed64)hit.point.x;
                    Fixed64 fixY = Fixed64.Zero;
                    Fixed64 fixZ = (Fixed64)hit.point.z;

                    Proto.OpData opData = new Proto.OpData();
                    opData.OpType = Proto.OpType.OpMove;
                    opData.UserId = AuthSocket.userInfo.UserId;
                    opData.MoveData = new Proto.OpMoveData();

                    opData.MoveData.X = fixX.m_rawValue;
                    opData.MoveData.Y = fixY.m_rawValue;
                    opData.MoveData.Z = fixZ.m_rawValue;

                    BattleSocket.FrameSyncRequest(opData);
                }
                else if (hit.collider.name == "Player")
                {
                    Debug.Log("点击角色");

                    //玩家攻击
                    //获取玩家UID
                    BattlePlayerObject playerObj = hit.collider.transform.parent.transform.GetComponent<BattlePlayerObject>();
                    long uid = playerObj.UserId;
                    Proto.OpData opData = new Proto.OpData();
                    opData.OpType = Proto.OpType.OpAttack;
                    opData.UserId = AuthSocket.userInfo.UserId;
                    opData.AttackData = new Proto.OpAttackData();
                    opData.AttackData.TargetUserId = uid;
                    BattleSocket.FrameSyncRequest(opData);
                }

            }
        }
    }
}
