# Frame Alignment 游戏服务器

基于 TypeScript 的游戏服务器框架，支持 WebSocket 实时通信、房间管理和玩家管理。

## 功能特性

- ✅ WebSocket 实时通信
- ✅ 房间管理系统
- ✅ 玩家管理
- ✅ 心跳检测
- ✅ 消息广播
- ✅ HTTP API 接口
- ✅ TypeScript 类型安全
- ✅ Protocol Buffers 支持（从 proto3 生成 TypeScript 接口）

## 项目结构

```
FrameAlignmentServer/
├── src/
│   ├── index.ts              # 入口文件
│   ├── server/
│   │   └── Server.ts         # 服务器主类
│   ├── network/
│   │   └── WebSocketServer.ts # WebSocket 服务器
│   ├── game/
│   │   ├── Player.ts         # 玩家类
│   │   ├── Room.ts           # 房间类
│   │   └── RoomManager.ts    # 房间管理器
│   └── types/
│       └── index.ts          # 类型定义
├── package.json
├── tsconfig.json
└── README.md
```

## 快速开始

### 安装依赖

```bash
npm install
```

### 生成 Proto 接口定义

```bash
npm run proto:generate
```

### 开发模式运行

```bash
npm run dev
```

### 构建项目

```bash
npm run build
```

### 生产模式运行

```bash
npm start
```

## 配置

默认端口为 `3000`，可以通过环境变量 `PORT` 修改：

```bash
PORT=8080 npm run dev
```

## API 接口

### HTTP API

- `GET /health` - 健康检查，返回服务器状态和在线玩家数
- `GET /rooms` - 获取所有房间列表

### WebSocket 消息协议

#### 客户端发送消息格式

```typescript
{
  type: MessageType,
  data?: any
}
```

#### 消息类型

- `connect` - 连接成功（服务器发送）
- `disconnect` - 断开连接
- `join_room` - 加入房间
  ```json
  {
    "type": "join_room",
    "data": {
      "roomId": "room_1"  // 可选，不提供则自动分配
    }
  }
  ```
- `leave_room` - 离开房间
- `game_message` - 游戏消息（会广播到房间内所有玩家）
- `heartbeat` - 心跳检测

#### 服务器响应示例

**连接成功**
```json
{
  "type": "connect",
  "data": {
    "playerId": "player_1",
    "message": "连接成功"
  }
}
```

**加入房间成功**
```json
{
  "type": "join_room",
  "data": {
    "room": {
      "id": "room_1",
      "name": "默认房间",
      "maxPlayers": 4,
      "players": ["player_1", "player_2"]
    },
    "message": "加入房间成功"
  }
}
```

## 使用示例

### WebSocket 客户端连接

```javascript
const ws = new WebSocket('ws://localhost:3000');

ws.onopen = () => {
  console.log('连接成功');
};

ws.onmessage = (event) => {
  const message = JSON.parse(event.data);
  console.log('收到消息:', message);
};

// 加入房间
ws.send(JSON.stringify({
  type: 'join_room',
  data: {}
}));

// 发送游戏消息
ws.send(JSON.stringify({
  type: 'game_message',
  data: {
    action: 'move',
    position: { x: 100, y: 200 }
  }
}));
```

## Protocol Buffers 支持

项目支持从 proto3 文件生成 TypeScript 接口定义。

### 使用步骤

1. 在 `proto/` 目录下创建或修改 `.proto` 文件
2. 运行生成脚本：
   ```bash
   npm run proto:generate
   ```
3. 生成的接口文件位于 `src/generated/` 目录
4. 在代码中导入使用：
   ```typescript
   import { MessageType, PlayerInfo, RoomInfo } from './generated';
   ```

### 监听模式

自动检测 proto 文件变化并重新生成：
```bash
npm run proto:watch
```

详细说明请参考 `scripts/README.md`

## 开发

### 代码结构说明

- **Server.ts**: 主服务器类，管理 HTTP 和 WebSocket 服务器
- **WebSocketServer.ts**: WebSocket 连接管理、消息处理和路由
- **Player.ts**: 玩家实体，管理玩家状态和连接
- **Room.ts**: 房间实体，管理房间内的玩家和消息广播
- **RoomManager.ts**: 房间管理器，负责房间的创建、查找和删除
- **scripts/**: Proto 生成脚本目录

### 扩展开发

1. **添加新的消息类型**: 
   - 方式1: 在 `src/types/index.ts` 中添加新的 `MessageType`
   - 方式2: 在 `proto/` 目录下定义 proto 文件，然后运行 `npm run proto:generate`
2. **添加消息处理**: 在 `WebSocketServer.ts` 的 `handleMessage` 方法中添加处理逻辑
3. **扩展游戏逻辑**: 在 `src/game/` 目录下添加新的游戏逻辑类

## 许可证

MIT

