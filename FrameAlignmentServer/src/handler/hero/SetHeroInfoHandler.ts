import { ClientObj } from "../../core/ClientObj";
import { proto } from "../../generated";
import { db } from "../../database/Database";

export function handleSetUseHero(client: ClientObj, ctx: Uint8Array): Uint8Array {
    const request = proto.SetUseHeroRequest.decode(ctx);
    const heroId = request.hero_id ?? 1001;

    // TODO: 可以在这里添加英雄 ID 合法性校验

    db.setUseHeroId(client.userId!, heroId);

    const response: proto.ISetUseHeroResponse = {
        base: {
            code: proto.StatusCode.STATUS_SUCCESS,
            id: proto.ApiId.API_SET_USE_HERO_RESP,
            message: '设置成功',
        },
        heroInfo: {
            use_hero_id: heroId,
        },
    };
    return proto.SetUseHeroResponse.encode(response).finish();
}

