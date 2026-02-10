import { ClientObj } from "../../core/ClientObj";
import { proto } from "../../generated";

/**
 * 玩家连接状态
 */
export enum PlayerConnectionState {
    /** 已连接 */
    CONNECTED = 'CONNECTED',
    /** 已断开（等待重连） */
    DISCONNECTED = 'DISCONNECTED',
    /** 已离开（主动退出） */
    LEFT = 'LEFT',
}

/**
 * 战斗玩家类
 * 表示参与战斗的玩家，包含玩家信息和当前帧的操作输入
 */
export class BattlePlayer {
    /** 玩家 ID（用户 ID） */
    public readonly userId: number;

    /** 玩家昵称 */
    public readonly nickname: string;

    /** 使用的英雄 ID */
    public readonly heroId: number;

    /** 客户端连接对象 */
    private _client: ClientObj | null;

    /** 连接状态 */
    private _connectionState: PlayerConnectionState = PlayerConnectionState.CONNECTED;

    /** 当前帧的操作输入列表 */
    private _pendingInputs: proto.IOpData[] = [];

    /** 最后活跃时间 */
    private _lastActiveTime: number = Date.now();

    /** 阵营（1 或 2） */
    public camp: number = 0;

    /** 是否已准备（场景资源加载完成） */
    private _isReady: boolean = false;

    constructor(userId: number, nickname: string, heroId: number, client: ClientObj) {
        this.userId = userId;
        this.nickname = nickname;
        this.heroId = heroId;
        this._client = client;
    }

    /**
     * 获取客户端连接
     */
    get client(): ClientObj | null {
        return this._client;
    }

    /**
     * 获取连接状态
     */
    get connectionState(): PlayerConnectionState {
        return this._connectionState;
    }

    /**
     * 是否在线
     */
    get isOnline(): boolean {
        return this._connectionState === PlayerConnectionState.CONNECTED && this._client !== null;
    }

    /**
     * 获取最后活跃时间
     */
    get lastActiveTime(): number {
        return this._lastActiveTime;
    }

    /**
     * 更新活跃时间
     */
    updateActiveTime(): void {
        this._lastActiveTime = Date.now();
    }

    /**
     * 添加操作输入
     * @param opData 操作数据
     */
    addInput(opData: proto.IOpData): void {
        // 设置 user_id
        opData.user_id = this.userId;
        this._pendingInputs.push(opData);
        this.updateActiveTime();
    }

    /**
     * 获取并清空当前帧的所有操作输入
     * @returns 操作输入列表
     */
    consumeInputs(): proto.IOpData[] {
        const inputs = this._pendingInputs;
        this._pendingInputs = [];
        return inputs;
    }

    /**
     * 发送消息给客户端
     * @param buffer 消息数据
     */
    send(buffer: Uint8Array): void {
        if (this._client && this.isOnline) {
            this._client.send(buffer);
        }
    }

    /**
     * 处理断线
     */
    onDisconnect(): void {
        this._connectionState = PlayerConnectionState.DISCONNECTED;
        this._client = null;
    }

    /**
     * 处理重连
     * @param client 新的客户端连接
     */
    onReconnect(client: ClientObj): void {
        this._client = client;
        this._connectionState = PlayerConnectionState.CONNECTED;
        this.updateActiveTime();
    }

    /**
     * 处理主动离开
     */
    onLeave(): void {
        console.log(`[BattlePlayer] 玩家 ${this.nickname}(${this.userId}) 主动离开，之前状态: ${this._connectionState}`);
        this._connectionState = PlayerConnectionState.LEFT;
        this._client = null;
        this._isReady = false;
        console.log(`[BattlePlayer] 玩家 ${this.nickname}(${this.userId}) 离开完成`);
    }

    /**
     * 是否已准备
     */
    get isReady(): boolean {
        return this._isReady;
    }

    /**
     * 设置准备状态
     */
    setReady(): void {
        this._isReady = true;
    }
}
