import { ClientObj } from "../../core/ClientObj";
import { proto } from "../../generated";
import { db } from "../../database/Database";
import * as bcrypt from "bcryptjs";

export function handleRegister(_client: ClientObj, ctx: Uint8Array): Uint8Array {
    const request = proto.RegisterRequest.decode(ctx);
    console.log('注册请求:', { account: request.account, nickname: request.nickname });

    // 参数校验
    if (!request.account || request.account.trim().length < 3) {
        return errorResponse(proto.StatusCode.STATUS_INVALID_PARAMS, '账号长度不能少于3个字符');
    }
    if (!request.password || request.password.length < 6) {
        return errorResponse(proto.StatusCode.STATUS_INVALID_PARAMS, '密码长度不能少于6个字符');
    }
    if (!request.nickname || request.nickname.trim().length === 0) {
        return errorResponse(proto.StatusCode.STATUS_INVALID_PARAMS, '昵称不能为空');
    }

    // 检查账号是否已存在
    if (db.findUserByAccount(request.account)) {
        return errorResponse(proto.StatusCode.STATUS_USER_ALREADY_EXIST, '账号已存在');
    }

    // 密码加密并创建用户
    const hashedPassword = bcrypt.hashSync(request.password, 10);
    const user = db.createUser(request.account.trim(), hashedPassword, request.nickname.trim());
    
    console.log('用户注册成功:', { id: user.id, account: user.account });

    // 生成 token
    const token = `${user.id}_${Date.now()}_${Math.random().toString(36).slice(2)}`;

    const response: proto.IRegisterResponse = {
        base: {
            code: proto.StatusCode.STATUS_SUCCESS,
            id: proto.ApiId.API_REGISTER_RESP,
            message: '注册成功',
        },
        user_info: {
            user_id: user.id,
            nickname: user.nickname,
            create_time: user.create_time,
            last_login_time: user.last_login_time,
        },
        token,
    };
    return proto.RegisterResponse.encode(response).finish();
}

function errorResponse(code: proto.StatusCode, message: string): Uint8Array {
    return proto.RegisterResponse.encode({
        base: { code, id: proto.ApiId.API_REGISTER_RESP, message },
    }).finish();
}
