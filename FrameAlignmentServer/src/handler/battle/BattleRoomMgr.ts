import { BattleRoom } from "./BattleRoom";
import { BattlePlayer } from "./BattlePlayer";
import { BattleConfig, BattleState } from "./BattleConfig";
import { ClientObj } from "../../core/ClientObj";
import { proto } from "../../generated";

/**
 * 战斗房间管理器
 * 直接使用房间进行匹配，不使用队列
 */
export class BattleRoomMgr {
    private static _instance: BattleRoomMgr;

    /** 所有房间 */
    private _rooms: Map<string, BattleRoom> = new Map();

    /** 玩家所在房间映射 */
    private _playerRoomMap: Map<number, string> = new Map();

    /** 房间 ID 计数器 */
    private _roomIdCounter: number = 1;

    private constructor() {}

    /**
     * 获取单例实例
     */
    static get Ins(): BattleRoomMgr {
        if (!this._instance) {
            this._instance = new BattleRoomMgr();
        }
        return this._instance;
    }

    /**
     * 生成新的房间 ID
     */
    private generateRoomId(): string {
        return `${BattleConfig.room.idPrefix}${this._roomIdCounter++}`;
    }

    /**
     * 创建新房间
     */
    createRoom(): BattleRoom {
        const roomId = this.generateRoomId();
        const room = new BattleRoom(roomId);
        this._rooms.set(roomId, room);
        return room;
    }

    /**
     * 获取房间
     */
    getRoom(roomId: string): BattleRoom | undefined {
        return this._rooms.get(roomId);
    }

    /**
     * 获取玩家所在的房间
     */
    getPlayerRoom(userId: number): BattleRoom | undefined {
        const roomId = this._playerRoomMap.get(userId);
        if (roomId) {
            return this._rooms.get(roomId);
        }
        return undefined;
    }

    /**
     * 查找等待中的房间（未满且状态为 WAITING）
     */
    private findWaitingRoom(): BattleRoom | undefined {
        for (const room of this._rooms.values()) {
            if (room.state === BattleState.WAITING && !room.isFull) {
                return room;
            }
        }
        return undefined;
    }

    /**
     * 玩家加入匹配
     * 查找等待中的房间，如果找到就加入，否则创建新房间
     */
    joinMatch(userId: number, nickname: string, heroId: number, client: ClientObj): BattleRoom | null {
        // 检查是否已在房间中
        const existingRoom = this.getPlayerRoom(userId);
        if (existingRoom) {
            return null;
        }

        // 查找等待中的房间
        let room = this.findWaitingRoom();
        
        // 如果没找到等待中的房间，创建新房间
        if (!room) {
            room = this.createRoom();
        }

        // 创建玩家对象并加入房间
        const player = new BattlePlayer(userId, nickname, heroId, client);
        if (room.addPlayer(player)) {
            this._playerRoomMap.set(userId, room.roomId);

            // 如果房间已满，通知所有玩家匹配成功
            if (room.isFull) {
                this.notifyMatchSuccess(room);
            }

            return room;
        }

        return null;
    }

    /**
     * 玩家取消匹配（离开房间）
     */
    cancelMatch(userId: number): boolean {
        return this.playerLeave(userId);
    }

    /**
     * 检查玩家是否在匹配队列中（实际是检查是否在房间中）
     */
    isInMatchQueue(userId: number): boolean {
        const room = this.getPlayerRoom(userId);
        return room !== undefined && room.state === BattleState.WAITING;
    }

    /**
     * 玩家离开房间
     */
    playerLeave(userId: number): boolean {
        console.log(`[BattleRoomMgr] 玩家 ${userId} 请求离开房间`);
        
        const roomId = this._playerRoomMap.get(userId);
        if (!roomId) {
            console.warn(`[BattleRoomMgr] 玩家 ${userId} 不在房间映射中`);
            return false;
        }

        console.log(`[BattleRoomMgr] 玩家 ${userId} 在房间 ${roomId} 中`);
        
        const room = this._rooms.get(roomId);
        if (!room) {
            console.error(`[BattleRoomMgr] 房间 ${roomId} 不存在，但玩家映射存在，清理映射`);
            this._playerRoomMap.delete(userId);
            return false;
        }

        console.log(`[BattleRoomMgr] 房间 ${roomId} 存在，当前状态: ${room.state}，当前人数: ${room.playerCount}`);
        
        room.removePlayer(userId);
        
        // 如果房间为空，销毁房间
        if (room.playerCount === 0) {
            console.log(`[BattleRoomMgr] 房间 ${roomId} 已为空，准备销毁`);
            this.destroyRoom(roomId);
        } else {
            console.log(`[BattleRoomMgr] 房间 ${roomId} 还有 ${room.playerCount} 人，不销毁`);
        }
        
        this._playerRoomMap.delete(userId);
        console.log(`[BattleRoomMgr] 玩家 ${userId} 已从房间映射中移除`);
        
        return true;
    }

    /**
     * 处理玩家断线
     * 如果玩家在匹配中（WAITING或COUNTDOWN状态），直接取消匹配
     * 如果玩家在战斗中（PLAYING状态），标记为断线等待重连
     * 如果战斗中掉线后房间没有在线玩家了，踢出所有玩家并销毁房间
     */
    playerDisconnect(userId: number): void {
        console.log(`[断线处理] 玩家 ${userId} 断线`);
        
        const room = this.getPlayerRoom(userId);
        if (!room) {
            console.log(`[断线处理] 玩家 ${userId} 不在任何房间中`);
            return;
        }

        console.log(`[断线处理] 玩家 ${userId} 在房间 ${room.roomId} 中，房间状态: ${room.state}`);

        // 如果玩家在匹配中或倒计时中（WAITING或COUNTDOWN状态），直接取消匹配
        if (room.state === BattleState.WAITING || room.state === BattleState.COUNTDOWN) {
            console.log(`[断线处理] 玩家 ${userId} 在匹配/倒计时中断线，取消匹配`);
            this.playerLeave(userId);
            return;
        }

        // 如果玩家在战斗中，标记为断线（等待重连）
        if (room.state === BattleState.PLAYING) {
            const player = room.getPlayer(userId);
            if (player) {
                player.onDisconnect();
                console.log(`[断线处理] 玩家 ${userId} 在战斗中断线，标记为断线状态`);
                
                // 检查房间中是否还有在线玩家
                const hasOnlinePlayer = room.players.some(p => p.isOnline);
                console.log(`[断线处理] 房间 ${room.roomId} 中是否还有在线玩家: ${hasOnlinePlayer}`);
                
                if (!hasOnlinePlayer) {
                    console.log(`[断线处理] 房间 ${room.roomId} 中没有在线玩家，踢出所有玩家并销毁房间`);
                    // 获取所有玩家ID（在销毁前）
                    const allPlayerIds = room.players.map(p => p.userId);
                    console.log(`[断线处理] 房间 ${room.roomId} 中的所有玩家: [${allPlayerIds.join(', ')}]`);
                    
                    // 先结束战斗（停止所有定时器）
                    room.endBattle();
                    console.log(`[断线处理] 房间 ${room.roomId} 战斗已结束`);
                    
                    // 踢出所有玩家（从房间映射中移除）
                    for (const playerId of allPlayerIds) {
                        this._playerRoomMap.delete(playerId);
                        console.log(`[断线处理] 已从房间映射中移除玩家 ${playerId}`);
                    }
                    
                    // 销毁房间
                    this.destroyRoom(room.roomId);
                    console.log(`[断线处理] 房间 ${room.roomId} 已销毁`);
                }
            }
        }
    }

    /**
     * 处理玩家重连
     */
    playerReconnect(userId: number, client: ClientObj): boolean {
        const room = this.getPlayerRoom(userId);
        if (room && room.state === BattleState.PLAYING) {
            const player = room.getPlayer(userId);
            if (player) {
                player.onReconnect(client);
                return true;
            }
        }
        return false;
    }

    /**
     * 销毁房间
     */
    destroyRoom(roomId: string): void {
        const room = this._rooms.get(roomId);
        if (room) {
            // 清除玩家的房间映射
            for (const player of room.players) {
                this._playerRoomMap.delete(player.userId);
            }
            room.destroy();
            this._rooms.delete(roomId);
        }
    }

    /**
     * 通知匹配成功
     */
    private notifyMatchSuccess(room: BattleRoom): void {
        // 构建玩家信息列表
        const playersInfo: proto.IPlayerInfo[] = room.players.map(player => ({
            user_id: player.userId,
            nickname: player.nickname,
            hero_id: player.heroId,
            camp: player.camp, // 第一位玩家为1，第二位玩家为2
        }));

        // 向所有玩家发送匹配成功通知
        for (const player of room.players) {
            const response: proto.IMatchSuccessResponse = {
                base: {
                    code: proto.StatusCode.STATUS_SUCCESS,
                    id: proto.ApiId.API_MATCH_SUCCESS_RESP,
                    message: '匹配成功！',
                },
                room_id: room.roomId,
                players: playersInfo,
            };
            const buffer = proto.MatchSuccessResponse.encode(response).finish();
            player.send(buffer);
        }
    }
}
