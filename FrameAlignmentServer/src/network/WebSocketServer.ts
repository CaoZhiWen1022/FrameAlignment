import { WebSocketServer as WS } from 'ws';
import { Router } from '../core/Router';
import { ClientObj } from '../core/ClientObj';
import { proto } from '../generated';
import { BattleRoomMgr } from '../handler/battle/BattleRoomMgr';

export class WebSocketServer {
  private wss: WS;
  private router: Router;
  private clients: Map<string, ClientObj> = new Map();
  constructor(port: number) {
    this.wss = new WS({ port });
    console.log(`WebSocket服务器已启动: ws://localhost:${port}`);

    //启用路由
    this.router = new Router();
    this.router.registerAll();

    this.start();
  }


  start(): void {
    this.wss.on('connection', (ws) => {
      console.log('客户端连接');
      let client = new ClientObj(ws as any);
      this.clients.set(ws.url, client);
      ws.on('message', async (message) => {
        const buffer = message as Uint8Array;
        // 解析 CommonRequest 并获取 apiId
        try {
          const commonRequest = proto.CommonRequest.decode(buffer);
          const apiId = commonRequest.base?.id;

          // 记录请求日志（仅对关键API）
          if (apiId === proto.ApiId.API_CANCEL_MATCH_REQ) {
            console.log(`[WebSocket] 收到取消匹配请求，客户端认证状态: ${client.isAuthenticated}, userId: ${client.userId}`);
          }

          // 检查是否需要认证
          if (apiId && !this.router.isPublicApi(apiId) && !client.isAuthenticated) {
            if (apiId === proto.ApiId.API_CANCEL_MATCH_REQ) {
              console.warn(`[WebSocket] 取消匹配请求失败: 客户端未认证`);
            }
            const errorResponse = proto.CommonResponse.create({
              base: {
                code: proto.StatusCode.STATUS_UNAUTHORIZED,
                id: apiId,
                message: '请先登录',
              },
            });
            client.send(proto.CommonResponse.encode(errorResponse).finish());
            return;
          }

          const handler = this.router.getHandler(apiId);
          if (handler) {
            let resp = await handler(client, buffer);
            client.send(resp);
            if (apiId === proto.ApiId.API_CANCEL_MATCH_REQ) {
              console.log(`[WebSocket] 取消匹配请求处理完成，已发送响应`);
            }
          } else {
            if (apiId === proto.ApiId.API_CANCEL_MATCH_REQ) {
              console.error(`[WebSocket] 取消匹配请求失败: 未找到处理程序`);
            }
            const commonResponse = proto.CommonResponse.create({
              base: {
                code: proto.StatusCode.STATUS_INTERNAL_ERROR,
                id: apiId,
                message: '未找到处理程序',
              },
            });
            let buffer = proto.CommonResponse.encode(commonResponse).finish();
            client.send(buffer);
          }
        } catch (error) {
          console.error('解析 CommonRequest 失败:', error);
          if (error instanceof Error) {
            console.error('错误堆栈:', error.stack);
          }
        }
      });
      ws.on('close', () => {
        console.log('客户端断开连接');
        // 处理玩家断线
        if (client.isAuthenticated && client.userId) {
          BattleRoomMgr.Ins.playerDisconnect(client.userId);
        }
        this.clients.delete(ws.url);
      });
    });
  }

  stop(): void {
    this.wss.close();
  }
}
