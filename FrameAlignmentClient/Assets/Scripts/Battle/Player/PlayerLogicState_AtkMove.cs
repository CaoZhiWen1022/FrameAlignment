using FixedMathSharp;
using Proto;
using System;

public class PlayerLogicState_AtkMove : PlayerLogicState_Base
{

    long targetUid;

    public PlayerLogicState_AtkMove(PlayerLogicState state, LogicData logicData, Action<PlayerLogicState, object> action) : base(state, logicData, action)
    {
    }


    public override void OnEnter(StateMachine machine, IState prevState, object param)
    {
        OpAttackData data = param as OpAttackData;
        targetUid = data.TargetUserId;
    }

    public override void OnLeave(IState nextState, object param)
    {
        base.OnLeave(nextState, param);
    }

    public override void OnUpdate()
    {
        Move();
    }

    void Move()
    {
        //开始时检测一下
        if (AtkDistanceCheck())
        {
            SwitchState(PlayerLogicState.atk, targetUid);
            return;
        }

        var curPos = logicData.posV3;
        var targetPos = FrameSyncMgr.ins.GetPlayerPosByUid(targetUid);
        Vector2d start = new Vector2d(curPos.x, curPos.z);
        Vector2d target = new Vector2d(targetPos.x, targetPos.z);
        var movePathV2s = AStarManager.Instance.FindPath(start, target);
        if (movePathV2s == null || movePathV2s.Count == 0)
        {
            //没有移动路径？？
            return;
        }


        // 这一帧能走的固定距离
        Fixed64 remainingDistance = logicData.playerData.speed * ConfigMgr.battleCfg.FixedDeltaTime;

        // 消耗式位移：如果剩余距离足以到达当前拐点，则直接转向下一个点
        while (remainingDistance > Fixed64.Zero && movePathV2s.Count > 0)
        {
            Vector2d nextV2 = movePathV2s[0];
            Vector3d nextV3 = new Vector3d(nextV2.x, logicData.posV3.y, nextV2.y);
            Fixed64 dist = Vector3d.Distance(logicData.posV3, nextV3);

            if (dist <= remainingDistance)
            {
                logicData.posV3 = nextV3;
                remainingDistance -= dist;
                movePathV2s.RemoveAt(0);
            }
            else
            {
                Vector3d moveDir = nextV3 - logicData.posV3;
                moveDir.Normalize();
                logicData.posV3 += moveDir * remainingDistance;

                // 计算转向 (使用Atan2确保全网一致)
                UpdateRotation(moveDir);
                remainingDistance = Fixed64.Zero;
            }
        }
        //结束时检测一下
        if (AtkDistanceCheck())
        {
            SwitchState(PlayerLogicState.atk, targetUid);
            return;
        }

    }

    bool AtkDistanceCheck()
    {
        //开始时检测一下
        var curPos = logicData.posV3;
        var targetPos = FrameSyncMgr.ins.GetPlayerPosByUid(targetUid);
        var commonAtkDis = logicData.playerData.commonAtkDistance;
        var dis = Vector3d.Distance(curPos, targetPos);
        return dis <= commonAtkDis;
    }

    void UpdateRotation(Vector3d moveDir)
    {
        double dx = (double)moveDir.x;
        double dz = (double)moveDir.z;
        if (Math.Abs(dx) > 0.0001 || Math.Abs(dz) > 0.0001)
        {
            double rad = Math.Atan2(dx, dz);
            logicData.rotate.y = (Fixed64)(rad * (180 / Math.PI));
        }
    }
}
