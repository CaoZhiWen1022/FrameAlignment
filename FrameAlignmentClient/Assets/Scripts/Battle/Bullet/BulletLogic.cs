using FixedMathSharp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletLogic : ObjectLogicBase
{

    private Fixed64 moveSpeed = (Fixed64)15;

    long targetUid;

    Fixed64 _moveThreshold = (Fixed64)0.3;

    public BulletLogic(Vector3d posV3, Vector3d rotate, long targetUid) : base(posV3, rotate)
    {
        this.targetUid = targetUid;
    }

    public override void OnUpdate()
    {
        if (isDispose) return;
        MoveToTarget();
    }

    private void MoveToTarget()
    {
        var curPos = posV3;
        var targetPos = FrameSyncMgr.ins.GetPlayerPosByUid(targetUid);
        targetPos.y = curPos.y;
        Fixed64 distance = Vector3d.Distance(posV3, targetPos);
        if (CheckHarm())
        {
            return;
        }

        Vector3d moveDir = targetPos - posV3;
        moveDir.Normalize();
        Fixed64 frameMoveDistance = moveSpeed * ConfigMgr.battleCfg.FixedDeltaTime;
        if (frameMoveDistance > distance)
        {
            frameMoveDistance = distance;
        }
        posV3 += moveDir * frameMoveDistance;

        CheckHarm();
    }

    private bool CheckHarm()
    {
        if (isDispose) return true;
        var curPos = posV3;
        var targetPos = FrameSyncMgr.ins.GetPlayerPosByUid(targetUid);
        targetPos.y = curPos.y;
        Fixed64 distance = Vector3d.Distance(posV3, targetPos);
        if (distance < _moveThreshold)
        {
            posV3 = targetPos;
            //触发伤害且销毁自身
            isDispose = true;
            FrameSyncMgr.ins.OnBulletHit(targetUid, (Fixed64)100);
            return true;
        }
        return false;

    }
}
