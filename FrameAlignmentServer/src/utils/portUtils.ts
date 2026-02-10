import { execSync } from 'child_process';

/**
 * 检查端口是否被占用，如果被占用则终止占用进程
 * 仅在 Windows 系统上有效
 */
export function killProcessOnPort(port: number): void {
  try {
    // 查找占用端口的进程
    const result = execSync(`netstat -ano | findstr :${port}`, { encoding: 'utf-8' });
    const lines = result.split('\n').filter(line => line.includes('LISTENING'));
    
    if (lines.length === 0) {
      console.log(`端口 ${port} 未被占用`);
      return;
    }

    // 提取 PID
    const pids = new Set<string>();
    for (const line of lines) {
      const parts = line.trim().split(/\s+/);
      const pid = parts[parts.length - 1];
      if (pid && /^\d+$/.test(pid)) {
        pids.add(pid);
      }
    }

    // 终止进程
    for (const pid of pids) {
      try {
        execSync(`taskkill /F /PID ${pid}`, { encoding: 'utf-8' });
        console.log(`✓ 已终止占用端口 ${port} 的进程 (PID: ${pid})`);
      } catch (e) {
        // 进程可能已经结束
      }
    }
  } catch (error) {
    // 端口未被占用时 findstr 会返回错误码
    console.log(`端口 ${port} 可用`);
  }
}

/**
 * 检查是否为开发模式
 */
export function isDev(): boolean {
  return process.env.NODE_ENV !== 'production';
}

