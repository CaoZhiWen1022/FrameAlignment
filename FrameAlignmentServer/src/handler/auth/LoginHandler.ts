import { ClientObj } from "../../core/ClientObj";
import { proto } from "../../generated";
import { db } from "../../database/Database";
import * as bcrypt from "bcryptjs";

export function handleLogin(client: ClientObj, ctx: Uint8Array): Uint8Array {
    const request = proto.LoginRequest.decode(ctx);
    console.log('登录请求:', { account: request.account, password: request.password });

    // 参数校验
    if (!request.account || request.account.trim().length === 0) {
        return errorResponse(proto.StatusCode.STATUS_INVALID_PARAMS, '账号不能为空');
    }
    if (!request.password || request.password.length === 0) {
        return errorResponse(proto.StatusCode.STATUS_INVALID_PARAMS, '密码不能为空');
    }

    // 查找用户
    const user = db.findUserByAccount(request.account);
    if (!user) {
        return errorResponse(proto.StatusCode.STATUS_USER_NOT_EXIST, '用户不存在');
    }

    // 验证密码
    console.log('调试信息:', {
        输入密码: request.password,
        数据库密码哈希: user.password,
        比较结果: bcrypt.compareSync(request.password, user.password)
    });
    if (!bcrypt.compareSync(request.password, user.password)) {
        return errorResponse(proto.StatusCode.STATUS_PASSWORD_ERROR, '密码错误');
    }

    // 更新最后登录时间
    db.updateLastLoginTime(user.id);

    // 设置客户端认证状态
    client.setAuthenticated(user.id);

    console.log('用户登录成功:', { id: user.id, account: user.account });

    const response: proto.ILoginResponse = {
        base: {
            code: proto.StatusCode.STATUS_SUCCESS,
            id: proto.ApiId.API_LOGIN_RESP,
            message: '登录成功',
        },
        user_info: {
            user_id: user.id,
            nickname: user.nickname,
            create_time: user.create_time,
            last_login_time: Date.now(),
        },
    };
    return proto.LoginResponse.encode(response).finish();
}

function errorResponse(code: proto.StatusCode, message: string): Uint8Array {
    return proto.LoginResponse.encode({
        base: { code, id: proto.ApiId.API_LOGIN_RESP, message },
    }).finish();
}

