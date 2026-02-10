## FrameAlignment

一个基于**帧同步**架构的多人游戏技术示例项目，包含客户端、服务端以及 UI 与协议工程，展示从底层网络、逻辑同步到 UI 表现的一整套实现方案。

### 功能与特点

- **完整的帧同步技术栈**
  - 服务端：`Node.js` + `WebSocket` Server + `SQLite` + `Proto` 协议
  - 客户端：`Unity` + 状态机架构 + `FGUI` UI 框架 + `A*` 寻路 + **定点数** 运算
- **端到端示例**
  - 从协议定义（Proto）
  - 到服务端逻辑与存储
  - 再到客户端渲染和交互
- **适合作为学习与二次开发基础**
  - 清晰的工程划分
  - 便于扩展新的游戏逻辑或玩法
  - 无复杂框架，旨在以最简单的方式实现帧同步的逻辑。

### 项目结构

- **`FrameAlignmentClient`**：Unity 客户端工程  
  - 游戏主逻辑  
  - 状态机管理  
  - A* 寻路与 
  - 定点数（第三方库）
  - 帧同步基础原理（逻辑层与表现层分离） 
- **`FrameAlignmentFGUI`**：FGUI UI 工程  
  - UI 界面与资源  
- **`FrameAlignmentServer`**：服务端工程  
  - 基于 Node.js 的帧同步服务器  
  - 使用 WebSocket 进行网络通信  
  - 使用 SQLite 进行数据存储  
- **`Proto`**：协议定义工程  
  - Proto 源文件  
  - 客户端与服务端共享的数据结构与消息协议  

### 技术栈

- **服务端**
  - Node.js  
  - WebSocket Server  
  - SQLite  
  - Proto / Protobuf  

- **客户端**
  - Unity  
  - 有限状态机（FSM）  
  - FGUI  
  - A* 寻路  
  - 定点数运算（用于保证多端逻辑一致性）  

### 环境与运行（示例说明，可按实际情况修改）

1. **克隆仓库**

   ```bash
   git clone https://github.com/CaoZhiWen1022/FrameAlignment.git
   ```

2. **服务端**

   - 进入 `FrameAlignmentServer` 目录  
   - 安装依赖并启动

   ```bash
   npm install
   npm run proto
   npm run start
   ```

3. **UI**
    - 使用fgui编辑器导出所有UI到Unity工程


4. **客户端**

   - 使用Tuanjie 1.8.0 打开 `FrameAlignmentClient` 工程  
   - 在 Editor 中运行，或打包到目标平台  
   - 确保客户端配置的服务器地址与端口与服务端一致  

### 适用人群

- 想要学习**帧同步**原理与实践的开发者  
- 想要了解 Unity + Node.js **端到端联调**流程的同学  
- 希望基于现成框架快速搭建自己多人游戏原型的独立开发者 / 学生  

### 定点数库-FixedMathSharp

https://github.com/mrdav30/FixedMathSharp.git

### 许可证

本项目默认采用 **MIT License** 开源。  


