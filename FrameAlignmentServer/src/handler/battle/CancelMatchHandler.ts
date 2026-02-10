import { ClientObj } from "../../core/ClientObj";
import { proto } from "../../generated";
import { BattleRoomMgr } from "./BattleRoomMgr";
import { BattleState } from "./BattleConfig";

/**
 * 处理取消匹配请求
 * 玩家在等待匹配或已匹配但未开始战斗时可以取消
 */
export function handleCancelMatch(client: ClientObj, _ctx: Uint8Array): Uint8Array {
    console.log(`[取消匹配] 收到取消匹配请求`);
    
    // 检查客户端认证状态
    if (!client.isAuthenticated) {
        console.error(`[取消匹配] 错误: 客户端未认证`);
        return errorResponse(proto.StatusCode.STATUS_UNAUTHORIZED, '请先登录');
    }

    const userId = client.userId;
    if (!userId) {
        console.error(`[取消匹配] 错误: userId 为空`);
        return errorResponse(proto.StatusCode.STATUS_INVALID_PARAMS, '用户ID无效');
    }

    console.log(`[取消匹配] 玩家 ${userId} 请求取消匹配`);

    // 获取玩家所在的房间
    const room = BattleRoomMgr.Ins.getPlayerRoom(userId);
    if (!room) {
        console.warn(`[取消匹配] 玩家 ${userId} 不在任何房间中`);
        return errorResponse(proto.StatusCode.STATUS_INVALID_PARAMS, '您不在任何房间中');
    }

    console.log(`[取消匹配] 玩家 ${userId} 在房间 ${room.roomId} 中，房间状态: ${room.state}，当前人数: ${room.playerCount}`);

    // 检查房间状态：只能在等待状态时取消匹配
    if (room.state !== BattleState.WAITING) {
        console.warn(`[取消匹配] 玩家 ${userId} 尝试在非等待状态取消匹配，当前状态: ${room.state}`);
        return errorResponse(proto.StatusCode.STATUS_INVALID_PARAMS, '战斗已开始，无法取消匹配');
    }

    // 离开房间（取消匹配）
    console.log(`[取消匹配] 开始执行玩家 ${userId} 离开房间操作`);
    const success = BattleRoomMgr.Ins.playerLeave(userId);
    
    if (!success) {
        console.error(`[取消匹配] 玩家 ${userId} 离开房间失败`);
        return errorResponse(proto.StatusCode.STATUS_INTERNAL_ERROR, '取消匹配失败');
    }

    console.log(`[取消匹配] 玩家 ${userId} 已成功取消匹配`);

    // 返回成功响应
    const response: proto.ICancelMatchResponse = {
        base: {
            code: proto.StatusCode.STATUS_SUCCESS,
            id: proto.ApiId.API_CANCEL_MATCH_RESP,
            message: '已取消匹配',
        },
    };
    return proto.CancelMatchResponse.encode(response).finish();
}

/**
 * 生成错误响应
 */
function errorResponse(code: proto.StatusCode, message: string): Uint8Array {
    return proto.CancelMatchResponse.encode({
        base: { code, id: proto.ApiId.API_CANCEL_MATCH_RESP, message },
    }).finish();
}
