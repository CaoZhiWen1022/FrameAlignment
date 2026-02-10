const fs = require('fs');
const path = require('path');
const { execSync } = require('child_process');

// 配置
const PROTO_DIR = path.join(__dirname, '../../Proto');
const OUTPUT_DIR = path.join(__dirname, '../src/generated');
const PROTO_OUTPUT_DIR = path.join(__dirname, '../src/generated/proto');

// 确保输出目录存在
function ensureDirectoryExists(dirPath) {
  if (!fs.existsSync(dirPath)) {
    fs.mkdirSync(dirPath, { recursive: true });
  }
}

// 使用 pbjs 生成静态 JavaScript 模块
function generateStaticModule() {
  try {
    console.log('使用 protobufjs-cli 生成静态模块...');
    
    // 获取所有 proto 文件
    const protoFiles = fs.readdirSync(PROTO_DIR)
      .filter(file => file.endsWith('.proto'))
      .map(file => path.join(PROTO_DIR, file));
    
    if (protoFiles.length === 0) {
      console.log('未找到 .proto 文件');
      return false;
    }
    
    const jsOutputPath = path.join(PROTO_OUTPUT_DIR, 'proto.js');
    const dtsOutputPath = path.join(PROTO_OUTPUT_DIR, 'proto.d.ts');
    
    // 生成 JavaScript 静态模块
    // -t static-module: 生成静态模块（包含编解码逻辑）
    // -w commonjs: 使用 CommonJS 模块语法（兼容 Node.js）
    // --keep-case: 保持字段名大小写
    // --force-number: 使用 number 类型而不是 Long
    const pbjsCmd = `node node_modules/protobufjs-cli/bin/pbjs -t static-module -w commonjs --keep-case --force-number -o "${jsOutputPath}" ${protoFiles.map(f => `"${f}"`).join(' ')}`;
    console.log(`执行: pbjs ...`);
    execSync(pbjsCmd, { stdio: 'inherit', cwd: path.join(__dirname, '..') });
    console.log(`✓ 生成 JavaScript 模块: ${jsOutputPath}`);
    
    // 生成 TypeScript 类型定义
    const pbtsCmd = `node node_modules/protobufjs-cli/bin/pbts -o "${dtsOutputPath}" "${jsOutputPath}"`;
    console.log(`执行: pbts ...`);
    execSync(pbtsCmd, { stdio: 'inherit', cwd: path.join(__dirname, '..') });
    console.log(`✓ 生成 TypeScript 定义: ${dtsOutputPath}`);
    
    return true;
  } catch (error) {
    console.error('生成失败:', error.message);
    return false;
  }
}

// 生成索引文件
function generateIndexFile() {
  const indexContent = `// 导出所有 protobuf 定义和编解码函数
export * from './proto/proto';
`;
  
  const indexPath = path.join(OUTPUT_DIR, 'index.ts');
  fs.writeFileSync(indexPath, indexContent);
  console.log(`✓ 生成索引文件: ${indexPath}`);
  
  // 同时在 proto 目录生成索引
  const protoIndexContent = `export * from './proto';
`;
  const protoIndexPath = path.join(PROTO_OUTPUT_DIR, 'index.ts');
  fs.writeFileSync(protoIndexPath, protoIndexContent);
  console.log(`✓ 生成 proto 索引文件: ${protoIndexPath}`);
}

// 清理旧的生成文件
function cleanOldFiles() {
  const filesToDelete = ['auth.ts', 'common.ts'];
  for (const file of filesToDelete) {
    const filePath = path.join(PROTO_OUTPUT_DIR, file);
    if (fs.existsSync(filePath)) {
      fs.unlinkSync(filePath);
      console.log(`✓ 删除旧文件: ${file}`);
    }
  }
}

// 生成所有
function generateAll() {
  console.log('开始生成 TypeScript 静态模块...\n');
  
  ensureDirectoryExists(OUTPUT_DIR);
  ensureDirectoryExists(PROTO_OUTPUT_DIR);

  if (!fs.existsSync(PROTO_DIR)) {
    console.error(`Proto 目录不存在: ${PROTO_DIR}`);
    console.log('请确保 Proto 目录存在于项目根目录的上级目录中');
    return;
  }

  // 清理旧文件
  cleanOldFiles();

  if (generateStaticModule()) {
    generateIndexFile();
    console.log('\n✓ 所有文件生成完成！');
    console.log('\n使用方式:');
    console.log('  import { RegisterRequest, RegisterResponse } from "./generated";');
    console.log('  ');
    console.log('  // 编码');
    console.log('  const buffer = RegisterRequest.encode(request).finish();');
    console.log('  ');
    console.log('  // 解码');
    console.log('  const decoded = RegisterRequest.decode(buffer);');
  }
}

// 监听模式
function watchMode() {
  console.log('监听模式已启动，监控 proto 文件变化...\n');
  
  if (!fs.existsSync(PROTO_DIR)) {
    console.error(`Proto 目录不存在: ${PROTO_DIR}`);
    return;
  }

  // 初始生成
  generateAll();

  // 监听文件变化
  fs.watch(PROTO_DIR, { recursive: true }, (eventType, filename) => {
    if (filename && filename.endsWith('.proto')) {
      console.log(`\n检测到变化: ${filename}`);
      generateAll();
    }
  });
}

// 主函数
function main() {
  const args = process.argv.slice(2);
  const isWatchMode = args.includes('--watch') || args.includes('-w');

  if (isWatchMode) {
    watchMode();
  } else {
    generateAll();
  }
}

main();
