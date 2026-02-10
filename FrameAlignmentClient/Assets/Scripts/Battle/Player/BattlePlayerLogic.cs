using FixedMathSharp;
using Proto;
using System;

public class BattlePlayerLogic
{

    private PlayerLogicStateMachine stateMachine;

    public LogicData logicData;
    public PlayerInfo playerInfo;

    public Vector3d PosV3 { get { return logicData.posV3; } }
    public Vector3d Rotate { get { return logicData.rotate; } }

    public long uid { get { return playerInfo.UserId; } }

    private Action<Fixed64> TakeHarmHandler;

    public void Init(Vector3d pos, Vector3d rotate, PlayerInfo playerData)
    {
        this.playerInfo = playerData;
        logicData = new LogicData();
        logicData.initPos = new Vector3d(pos.x, pos.y, pos.z);
        logicData.initRot = new Vector3d(rotate.x, rotate.y, rotate.z);
        logicData.posV3 = pos;
        logicData.rotate = rotate;
        logicData.playerData = new BattlePlayerData();
        logicData.playerData.InitData(playerData.HeroId);
        stateMachine = new PlayerLogicStateMachine(logicData, playerInfo.UserId);
        stateMachine.SwitchState(PlayerLogicState.idle, null);
    }



    public void SetFrameData(OpData opData)
    {
        if (opData.OpType == OpType.OpMove)
        {
            var moveData = opData.MoveData;
            Vector3d targetPosV3 = new Vector3d(Fixed64.FromRaw(moveData.X), Fixed64.FromRaw(moveData.Y), Fixed64.FromRaw(moveData.Z));
            stateMachine.SwitchState(PlayerLogicState.move, targetPosV3);
        }
        else if (opData.OpType == OpType.OpAttack)
        {
            var atkData = opData.AttackData;
            //æ‡¿ÎºÏ≤‚
            var curPos = logicData.posV3;
            var targetPos = FrameSyncMgr.ins.GetPlayerPosByUid(atkData.TargetUserId);
            var dis = Vector3d.Distance(curPos, targetPos);
            if (dis > logicData.playerData.commonAtkDistance)
            {
                stateMachine.SwitchState(PlayerLogicState.atkMove, atkData);
            }
            else
            {
                stateMachine.SwitchState(PlayerLogicState.atk, atkData.TargetUserId);
            }
        }
    }

    public void Update()
    {
        stateMachine.Update();
    }

    public void SetStateSwitchHandler(Action<PlayerLogicState> action)
    {
        stateMachine.StateSwitchHandler = action;
    }

    public void SetTakeHarmHandler(Action<Fixed64> action)
    {
        TakeHarmHandler = action;
    }

    public void TakeHarm(Fixed64 harmValue)
    {
        logicData.playerData.hp -= harmValue;
        //Õ®÷™‰÷»æ≤„œ‘ æ…À∫¶∆Æ¬‰
        TakeHarmHandler?.Invoke(harmValue);
        if ((int)logicData.playerData.hp <= 0)
        {
            //À¿Õˆ
            stateMachine.SwitchState(PlayerLogicState.die, null);
        }
    }
}

