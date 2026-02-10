import { ClientObj } from "../../core/ClientObj";
import { proto } from "../../generated";
import { db } from "../../database/Database";
import { BattleRoomMgr } from "./BattleRoomMgr";

/**
 * 处理匹配请求
 * 玩家加入匹配队列
 */
export function handleMatch(client: ClientObj, _ctx: Uint8Array): Uint8Array {
    const userId = client.userId!;
    
    // 获取玩家信息
    const user = db.findUserById(userId);
    if (!user) {
        return errorResponse(proto.StatusCode.STATUS_USER_NOT_EXIST, '用户不存在');
    }

    // 检查玩家是否已在房间中
    const existingRoom = BattleRoomMgr.Ins.getPlayerRoom(userId);
    if (existingRoom) {
        return errorResponse(proto.StatusCode.STATUS_INVALID_PARAMS, '您已在战斗中，无法匹配');
    }

    // 检查是否已在匹配队列中
    if (BattleRoomMgr.Ins.isInMatchQueue(userId)) {
        return errorResponse(proto.StatusCode.STATUS_INVALID_PARAMS, '您已在匹配中');
    }

    // 获取玩家使用的英雄 ID
    const heroId = db.getUseHeroId(userId);

    // 加入匹配（直接加入房间）
    const room = BattleRoomMgr.Ins.joinMatch(userId, user.nickname, heroId, client);
    
    if (!room) {
        return errorResponse(proto.StatusCode.STATUS_INTERNAL_ERROR, '加入匹配失败');
    }

    console.log(`[匹配] 玩家 ${user.nickname}(${userId}) 加入房间 ${room.roomId}，当前人数: ${room.playerCount}`);

    // 返回成功响应
    const response: proto.IMatchResponse = {
        base: {
            code: proto.StatusCode.STATUS_SUCCESS,
            id: proto.ApiId.API_MATCH_RESP,
            message: room.isFull ? '匹配成功！' : '正在匹配中...',
        },
    };
    return proto.MatchResponse.encode(response).finish();
}

/**
 * 生成错误响应
 */
function errorResponse(code: proto.StatusCode, message: string): Uint8Array {
    return proto.MatchResponse.encode({
        base: { code, id: proto.ApiId.API_MATCH_RESP, message },
    }).finish();
}

