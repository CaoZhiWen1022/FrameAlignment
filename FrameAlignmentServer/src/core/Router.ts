import { proto } from "../generated";
import { handleRegister } from "../handler/auth/RegisterHandler";
import { handleLogin } from "../handler/auth/LoginHandler";
import { handleGetHeroInfo } from "../handler/hero/GetHeroInfoHandler";
import { handleSetUseHero } from "../handler/hero/SetHeroInfoHandler";
import { handleBattleReady } from "../handler/battle/BattleReadyHandler";
import { handleFrameSync } from "../handler/battle/FrameSyncHandler";
import { handleMatch } from "../handler/battle/MatchHandler";
import { handleCancelMatch } from "../handler/battle/CancelMatchHandler";
import { ClientObj } from "./ClientObj";

export class Router {

    private routes: Map<proto.ApiId, (client: ClientObj, ctx: Uint8Array) => Uint8Array>;

    // 不需要认证的 API 白名单
    private readonly publicApis: Set<proto.ApiId> = new Set([
        proto.ApiId.API_LOGIN_REQ,
        proto.ApiId.API_REGISTER_REQ,
    ]);

    constructor() {
        this.routes = new Map();
    }

    registerAll() {
        // 认证相关
        this.register(proto.ApiId.API_REGISTER_REQ, handleRegister);
        this.register(proto.ApiId.API_LOGIN_REQ, handleLogin);
        // 英雄相关
        this.register(proto.ApiId.API_GET_HERO_INFO_REQ, handleGetHeroInfo);
        this.register(proto.ApiId.API_SET_USE_HERO_REQ, handleSetUseHero);
        // 战斗相关
        this.register(proto.ApiId.API_MATCH_REQ, handleMatch);
        this.register(proto.ApiId.API_CANCEL_MATCH_REQ, handleCancelMatch);
        this.register(proto.ApiId.API_BATTLE_READY_REQ, handleBattleReady);
        this.register(proto.ApiId.API_FRAME_SYNC_REQ, handleFrameSync);
    }

    register(apiId: proto.ApiId, handler: (client: ClientObj, ctx: Uint8Array) => Uint8Array): void {
        this.routes.set(apiId, handler);
    }

    getHandler(apiId: proto.ApiId | null | undefined): ((client: ClientObj, ctx: Uint8Array) => Uint8Array) | undefined {
        if (apiId == null) return undefined;
        return this.routes.get(apiId);
    }

    isPublicApi(apiId: proto.ApiId): boolean {
        return this.publicApis.has(apiId);
    }
}
