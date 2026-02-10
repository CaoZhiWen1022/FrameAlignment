/**
 * 战斗配置文件
 * 只保留基础配置
 */

export const BattleConfig = {
    /**
     * 帧同步相关配置
     */
    frameSync: {
        /**
         * 逻辑帧率（每秒帧数）
         */
        frameRate: 15,

        /**
         * 帧间隔时间（毫秒）
         */
        get frameInterval(): number {
            return Math.floor(1000 / this.frameRate);
        },
    },

    /**
     * 匹配相关配置
     */
    match: {
        /**
         * 房间最大玩家数
         */
        maxPlayers: 2,

        /**
         * 匹配成功后的倒计时时间（秒）
         */
        countdownSeconds: 3,
    },

    /**
     * 房间相关配置
     */
    room: {
        /**
         * 房间 ID 前缀
         */
        idPrefix: 'ROOM_',
    },
};

/**
 * 战斗状态枚举
 */
export enum BattleState {
    /** 等待玩家加入 */
    WAITING = 'WAITING',
    /** 倒计时中 */
    COUNTDOWN = 'COUNTDOWN',
    /** 战斗进行中 */
    PLAYING = 'PLAYING',
    /** 战斗已结束 */
    FINISHED = 'FINISHED',
}
