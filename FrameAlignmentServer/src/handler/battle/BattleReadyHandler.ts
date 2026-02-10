import { ClientObj } from "../../core/ClientObj";
import { proto } from "../../generated";
import { BattleRoomMgr } from "./BattleRoomMgr";
import { BattleState } from "./BattleConfig";

/**
 * 处理准备请求
 * 玩家场景资源加载完成，可以开始战斗
 */
export function handleBattleReady(client: ClientObj, _ctx: Uint8Array): Uint8Array {
    const userId = client.userId!;
    
    // 获取玩家所在的房间
    const room = BattleRoomMgr.Ins.getPlayerRoom(userId);
    if (!room) {
        return errorResponse(proto.StatusCode.STATUS_INVALID_PARAMS, '您不在任何房间中');
    }

    // 检查房间状态（必须是等待状态）
    if (room.state !== BattleState.WAITING) {
        return errorResponse(proto.StatusCode.STATUS_INVALID_PARAMS, '房间状态不正确，无法准备');
    }

    // 检查玩家是否已经准备
    const player = room.getPlayer(userId);
    if (player && player.isReady) {
        return errorResponse(proto.StatusCode.STATUS_INVALID_PARAMS, '您已经准备过了');
    }

    // 设置玩家准备状态
    const allReady = room.playerReady(userId);
    
    console.log(`[准备] 玩家 ${userId} 已准备，房间 ${room.roomId}，所有玩家准备: ${allReady}`);

    // 返回成功响应
    const response: proto.IBattleReadyResponse = {
        base: {
            code: proto.StatusCode.STATUS_SUCCESS,
            id: proto.ApiId.API_BATTLE_READY_RESP,
            message: allReady ? '所有玩家已准备，即将开始倒计时' : '准备成功，等待其他玩家',
        },
    };
    return proto.BattleReadyResponse.encode(response).finish();
}

/**
 * 生成错误响应
 */
function errorResponse(code: proto.StatusCode, message: string): Uint8Array {
    return proto.BattleReadyResponse.encode({
        base: { code, id: proto.ApiId.API_BATTLE_READY_RESP, message },
    }).finish();
}

