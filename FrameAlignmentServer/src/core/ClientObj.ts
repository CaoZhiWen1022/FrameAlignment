/** 连接到服务器的客户端对象 */
export class ClientObj {

    private ws: WebSocket;
    private _userId: number | null = null;

    constructor(ws: WebSocket) {
        this.ws = ws;
    }

    /** 是否已认证 */
    get isAuthenticated(): boolean {
        return this._userId !== null;
    }

    /** 获取用户 ID */
    get userId(): number | null {
        return this._userId;
    }

    /** 设置认证状态 */
    setAuthenticated(userId: number): void {
        this._userId = userId;
    }

    send(buffer: Uint8Array): void {
        this.ws.send(buffer);
    }
}
