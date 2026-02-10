import { ClientObj } from "../../core/ClientObj";
import { proto } from "../../generated";
import { db } from "../../database/Database";

export function handleGetHeroInfo(client: ClientObj, _ctx: Uint8Array): Uint8Array {
    const useHeroId = db.getUseHeroId(client.userId!);

    const response: proto.IGetHeroInfoResponse = {
        base: {
            code: proto.StatusCode.STATUS_SUCCESS,
            id: proto.ApiId.API_GET_HERO_INFO_RESP,
            message: '获取成功',
        },
        heroInfo: {
            use_hero_id: useHeroId,
        },
    };
    return proto.GetHeroInfoResponse.encode(response).finish();
}

