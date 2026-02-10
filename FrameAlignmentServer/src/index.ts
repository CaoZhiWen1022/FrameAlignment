import { WebSocketServer } from './network/WebSocketServer';
import { db } from './database/Database';
import { killProcessOnPort, isDev } from './utils/portUtils';

const PORT = process.env.PORT ? parseInt(process.env.PORT) : 3000;

async function main() {
  // 开发模式下，自动释放被占用的端口
  if (isDev()) {
    killProcessOnPort(PORT);
  }

  // 初始化数据库
  await db.initialize();
  
  // 启动服务器
  const server = new WebSocketServer(PORT);

  // 优雅关闭
  process.on('SIGINT', () => {
    console.log('\n正在关闭服务器...');
    server.stop();
    process.exit(0);
  });

  process.on('SIGTERM', () => {
    console.log('\n正在关闭服务器...');
    server.stop();
    process.exit(0);
  });
}

main().catch(console.error);
