import { BattlePlayer } from "./BattlePlayer";
import { BattleConfig, BattleState } from "./BattleConfig";
import { proto } from "../../generated";

/**
 * 战斗房间类
 * 管理一场战斗的所有状态，包括玩家、帧同步、倒计时等
 */
export class BattleRoom {
    /** 房间 ID */
    public readonly roomId: string;

    /** 战斗 ID（数字形式，用于协议） */
    public readonly battleId: number;

    /** 房间内的玩家列表 */
    private _players: Map<number, BattlePlayer> = new Map();

    /** 当前战斗状态 */
    private _state: BattleState = BattleState.WAITING;

    /** 当前帧索引 */
    private _currentFrame: number = 0;

    /** 战斗开始时间 */
    private _startTime: number = 0;

    /** 帧同步定时器 */
    private _frameTimer: NodeJS.Timeout | null = null;

    /** 倒计时定时器 */
    private _countdownTimer: NodeJS.Timeout | null = null;

    /** 帧数据缓存（用于断线重连） */
    private _frameBuffer: proto.IFrameData[] = [];

    /** 房间创建时间 */
    public readonly createTime: number = Date.now();

    private static _battleIdCounter: number = 1000;

    constructor(roomId: string) {
        this.roomId = roomId;
        this.battleId = BattleRoom._battleIdCounter++;
    }

    /**
     * 获取当前状态
     */
    get state(): BattleState {
        return this._state;
    }

    /**
     * 获取当前帧索引
     */
    get currentFrame(): number {
        return this._currentFrame;
    }

    /**
     * 获取玩家数量
     */
    get playerCount(): number {
        return this._players.size;
    }

    /**
     * 房间是否已满
     */
    get isFull(): boolean {
        return this._players.size >= BattleConfig.match.maxPlayers;
    }

    /**
     * 获取所有玩家
     */
    get players(): BattlePlayer[] {
        return Array.from(this._players.values());
    }

    /**
     * 根据用户 ID 获取玩家
     */
    getPlayer(userId: number): BattlePlayer | undefined {
        return this._players.get(userId);
    }

    /**
     * 添加玩家到房间
     * @param player 玩家对象
     * @returns 是否添加成功
     */
    addPlayer(player: BattlePlayer): boolean {
        if (this.isFull) {
            console.log(`[房间${this.roomId}] 房间已满，无法加入`);
            return false;
        }

        if (this._state !== BattleState.WAITING) {
            console.log(`[房间${this.roomId}] 战斗已开始，无法加入`);
            return false;
        }

        // 设置阵营：第一位玩家为1，第二位玩家为2
        player.camp = this._players.size + 1;
        this._players.set(player.userId, player);
        console.log(`[房间${this.roomId}] 玩家 ${player.nickname}(${player.userId}) 加入房间，阵营: ${player.camp}，当前人数: ${this._players.size}`);

        return true;
    }

    /**
     * 移除玩家
     * @param userId 用户 ID
     */
    removePlayer(userId: number): void {
        console.log(`[房间${this.roomId}] 开始移除玩家 ${userId}，当前状态: ${this._state}，当前人数: ${this._players.size}`);
        
        const player = this._players.get(userId);
        if (!player) {
            console.warn(`[房间${this.roomId}] 玩家 ${userId} 不在房间中`);
            return;
        }

        console.log(`[房间${this.roomId}] 找到玩家 ${player.nickname}(${userId})，准备移除`);
        
        player.onLeave();
        this._players.delete(userId);
        console.log(`[房间${this.roomId}] 玩家 ${player.nickname}(${userId}) 已离开房间，剩余人数: ${this._players.size}`);

        // 如果战斗中有玩家离开，可能需要结束战斗
        if (this._state === BattleState.PLAYING && this._players.size < BattleConfig.match.maxPlayers) {
            console.log(`[房间${this.roomId}] 战斗中玩家离开，结束战斗`);
            this.endBattle();
        }
        
        // 如果房间在倒计时状态，需要停止倒计时
        if (this._state === BattleState.COUNTDOWN) {
            console.log(`[房间${this.roomId}] 倒计时中玩家离开，停止倒计时`);
            if (this._countdownTimer) {
                clearInterval(this._countdownTimer);
                this._countdownTimer = null;
            }
            // 重置状态为等待（如果还有玩家）或保持当前状态
            if (this._players.size > 0) {
                this._state = BattleState.WAITING;
                console.log(`[房间${this.roomId}] 房间状态重置为 WAITING`);
            }
        }
    }

    /**
     * 检查所有玩家是否已准备
     */
    allPlayersReady(): boolean {
        if (!this.isFull) {
            return false;
        }
        for (const player of this._players.values()) {
            if (!player.isReady) {
                return false;
            }
        }
        return true;
    }

    /**
     * 玩家准备完成
     * @param userId 用户 ID
     * @returns 是否所有玩家都已准备（如果是，则开始倒计时）
     */
    playerReady(userId: number): boolean {
        const player = this._players.get(userId);
        if (player) {
            player.setReady();
            console.log(`[房间${this.roomId}] 玩家 ${player.nickname}(${userId}) 已准备`);
            
            // 检查是否所有玩家都已准备
            if (this.allPlayersReady()) {
                this.startCountdown();
                return true;
            }
        }
        return false;
    }

    /**
     * 开始倒计时
     * 每秒发送一次倒计时（3、2、1），1结束后等待1秒再发送开始战斗
     */
    private startCountdown(): void {
        if (this._state !== BattleState.WAITING) {
            return; // 如果已经在倒计时或战斗中，不重复开始
        }

        this._state = BattleState.COUNTDOWN;
        console.log(`[房间${this.roomId}] 所有玩家已准备，开始倒计时...`);

        let countdown = BattleConfig.match.countdownSeconds;

        // 立即发送第一次倒计时（3）
        this.broadcastCountdown(countdown);

        this._countdownTimer = setInterval(() => {
            countdown--;
            if (countdown > 0) {
                // 发送倒计时（2、1）
                this.broadcastCountdown(countdown);
            } else {
                // 倒计时结束（1已发送完），清除定时器
                if (this._countdownTimer) {
                    clearInterval(this._countdownTimer);
                    this._countdownTimer = null;
                }
                // 1结束后等待1秒再发送开始战斗
                setTimeout(() => {
                    this.startBattle();
                }, 1000);
            }
        }, 1000);
    }

    /**
     * 广播倒计时
     * @param seconds 剩余秒数
     */
    private broadcastCountdown(seconds: number): void {
        const response: proto.IBattleCountdownResponse = {
            base: {
                code: proto.StatusCode.STATUS_SUCCESS,
                id: proto.ApiId.API_BATTLE_COUNTDOWN_RESP,
                message: `倒计时: ${seconds}`,
            },
            countdown: seconds,
        };
        const buffer = proto.BattleCountdownResponse.encode(response).finish();
        this.broadcast(buffer);
        console.log(`[房间${this.roomId}] 倒计时: ${seconds}`);
    }

    /**
     * 开始战斗
     */
    private startBattle(): void {
        this._state = BattleState.PLAYING;
        this._startTime = Date.now();
        this._currentFrame = 0;

        console.log(`[房间${this.roomId}] 战斗开始! battleId=${this.battleId}`);

        // 广播战斗开始消息
        const response: proto.IBattleStartResponse = {
            base: {
                code: proto.StatusCode.STATUS_SUCCESS,
                id: proto.ApiId.API_BATTLE_START_RESP,
                message: '战斗开始',
            },
            battle_id: this.battleId,
            frame_rate: BattleConfig.frameSync.frameRate,
            start_time: this._startTime,
        };
        const buffer = proto.BattleStartResponse.encode(response).finish();
        this.broadcast(buffer);

        // 启动帧同步定时器
        this.startFrameSync();
    }

    /**
     * 启动帧同步
     */
    private startFrameSync(): void {
        this._frameTimer = setInterval(() => {
            this.tick();
        }, BattleConfig.frameSync.frameInterval);
    }

    /**
     * 帧同步 tick
     * 收集所有玩家的输入并广播帧数据
     */
    private tick(): void {
        if (this._state !== BattleState.PLAYING) return;

        // 收集所有玩家的输入
        const allInputs: proto.IOpData[] = [];
        for (const player of this._players.values()) {
            const inputs = player.consumeInputs();
            allInputs.push(...inputs);
        }

        // 构建帧数据
        const frameData: proto.IFrameData = {
            frame_index: this._currentFrame,
            timestamp: Date.now(),
            op_data_list: allInputs,
        };

        // 缓存帧数据（简单限制大小）
        this._frameBuffer.push(frameData);
        if (this._frameBuffer.length > 300) {
            this._frameBuffer.shift();
        }

        // 广播帧数据
        const response: proto.IFrameDataResponse = {
            base: {
                code: proto.StatusCode.STATUS_SUCCESS,
                id: proto.ApiId.API_FRAME_DATA_RESP,
                message: '',
            },
            frame_data: frameData,
        };
        const buffer = proto.FrameDataResponse.encode(response).finish();
        this.broadcast(buffer);

        this._currentFrame++;
    }

    /**
     * 处理玩家输入
     * @param userId 用户 ID
     * @param opDataList 操作数据列表
     */
    handlePlayerInput(userId: number, opData: proto.IOpData): void {
        const player = this._players.get(userId);
        if (!player) {
            console.warn(`[房间${this.roomId}] 未找到玩家: ${userId}`);
            return;
        }

        player.addInput(opData);
    }

    /**
     * 结束战斗
     */
    endBattle(): void {
        this._state = BattleState.FINISHED;

        // 停止所有定时器
        if (this._frameTimer) {
            clearInterval(this._frameTimer);
            this._frameTimer = null;
        }
        if (this._countdownTimer) {
            clearInterval(this._countdownTimer);
            this._countdownTimer = null;
        }

        console.log(`[房间${this.roomId}] 战斗结束，共 ${this._currentFrame} 帧`);

        // TODO: 发送战斗结束消息，计算战斗结果
    }

    /**
     * 广播消息给房间内所有玩家
     * @param buffer 消息数据
     */
    broadcast(buffer: Uint8Array): void {
        for (const player of this._players.values()) {
            player.send(buffer);
        }
    }

    /**
     * 获取帧缓存（用于断线重连）
     * @param fromFrame 起始帧
     * @returns 帧数据列表
     */
    getFrameBuffer(fromFrame: number): proto.IFrameData[] {
        return this._frameBuffer.filter(f => (f.frame_index ?? 0) >= fromFrame);
    }

    /**
     * 销毁房间，清理资源
     */
    destroy(): void {
        this.endBattle();
        this._players.clear();
        this._frameBuffer = [];
        console.log(`[房间${this.roomId}] 房间已销毁`);
    }
}
