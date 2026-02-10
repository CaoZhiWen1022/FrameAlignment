import { ClientObj } from "../../core/ClientObj";
import { proto } from "../../generated";
import { BattleRoomMgr } from "./BattleRoomMgr";
import { BattleState } from "./BattleConfig";

/**
 * 处理帧同步请求
 * 客户端发送操作输入，服务器收集后在下一帧广播
 */
export function handleFrameSync(client: ClientObj, ctx: Uint8Array): Uint8Array {
    const userId = client.userId!;
    const request = proto.FrameSyncRequest.decode(ctx);

    // 获取玩家所在房间
    const room = BattleRoomMgr.Ins.getPlayerRoom(userId);
    if (!room) {
        return errorResponse(proto.StatusCode.STATUS_INVALID_PARAMS, '您不在任何战斗房间中');
    }

    // 检查战斗状态
    if (room.state !== BattleState.PLAYING) {
        return errorResponse(proto.StatusCode.STATUS_INVALID_PARAMS, '战斗尚未开始或已结束');
    }

    // 处理玩家输入
    const opData: proto.IOpData = request.op_data as proto.IOpData;
    room.handlePlayerInput(userId, opData);

    // 帧同步请求不需要立即返回数据，服务器会定时广播帧数据
    // 这里返回一个简单的确认响应
    const response: proto.ICommonResponse = {
        base: {
            code: proto.StatusCode.STATUS_SUCCESS,
            id: proto.ApiId.API_FRAME_SYNC_RESP,
            message: '',
        },
    };
    return proto.CommonResponse.encode(response).finish();
}

/**
 * 生成错误响应
 */
function errorResponse(code: proto.StatusCode, message: string): Uint8Array {
    return proto.CommonResponse.encode({
        base: { code, id: proto.ApiId.API_FRAME_SYNC_REQ, message },
    }).finish();
}

