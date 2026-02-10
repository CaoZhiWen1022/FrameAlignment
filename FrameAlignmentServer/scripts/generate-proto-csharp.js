const fs = require('fs');
const path = require('path');
const { execSync } = require('child_process');

// 配置
const PROTO_DIR = path.join(__dirname, '../../Proto');
// 默认输出到 Unity 项目的 Scripts/Proto 目录，可通过命令行参数修改
const DEFAULT_OUTPUT_DIR = path.join(__dirname, '../../FrameAlignmentClient/Assets/Scripts/Proto');
// 本地 protoc 路径（官方版本，支持 C#）
const LOCAL_PROTOC_PATH = 'G:\\protoc\\bin\\protoc.exe';

// 获取输出目录（支持命令行参数）
function getOutputDir() {
  const args = process.argv.slice(2);
  for (let i = 0; i < args.length; i++) {
    if (args[i] === '-o' || args[i] === '--output') {
      if (args[i + 1]) {
        return path.resolve(args[i + 1]);
      }
    }
  }
  return path.resolve(DEFAULT_OUTPUT_DIR);
}

// 确保目录存在
function ensureDirectoryExists(dirPath) {
  if (!fs.existsSync(dirPath)) {
    fs.mkdirSync(dirPath, { recursive: true });
  }
}

// 获取 grpc-tools 中的 protoc 路径
function getProtocPath() {
  try {
    // 尝试从 node_modules 中找到 grpc-tools
    const grpcToolsPath = require.resolve('grpc-tools');
    const grpcToolsDir = path.dirname(grpcToolsPath);
    
    // 根据操作系统选择正确的二进制文件
    const platform = process.platform;
    const arch = process.arch;
    
    let binaryName = 'protoc';
    if (platform === 'win32') {
      binaryName = 'protoc.exe';
    }
    
    // grpc-tools 的 protoc 在 bin 目录下
    const protocPath = path.join(grpcToolsDir, 'bin', binaryName);
    
    if (fs.existsSync(protocPath)) {
      return protocPath;
    }
    
    // 备用路径
    const altPath = path.join(grpcToolsDir, '..', 'grpc-tools', 'bin', binaryName);
    if (fs.existsSync(altPath)) {
      return altPath;
    }

    return null;
  } catch (error) {
    return null;
  }
}

// 检查 protoc 是否可用
function findProtoc() {
  // 1. 优先使用本地配置的 protoc 路径（官方版本，支持 C#）
  if (fs.existsSync(LOCAL_PROTOC_PATH)) {
    try {
      const versionOutput = execSync(`"${LOCAL_PROTOC_PATH}" --version`, { encoding: 'utf-8', stdio: 'pipe' });
      console.log(`使用本地 protoc: ${LOCAL_PROTOC_PATH}`);
      console.log(`版本: ${versionOutput.trim()}`);
      return LOCAL_PROTOC_PATH;
    } catch (error) {
      console.warn(`本地 protoc 无法运行: ${LOCAL_PROTOC_PATH}`);
    }
  }

  // 2. 尝试系统 PATH 中的 protoc
  try {
    const versionOutput = execSync('protoc --version', { encoding: 'utf-8', stdio: 'pipe' });
    console.log(`使用系统 PATH 中的 protoc: ${versionOutput.trim()}`);
    return 'protoc';
  } catch (error) {
    // 系统中没有 protoc
  }
  
  // 3. 备用：尝试从 grpc-tools 获取（注意：可能不支持 C#）
  const grpcProtoc = getProtocPath();
  if (grpcProtoc) {
    console.log(`使用 grpc-tools 中的 protoc: ${grpcProtoc}`);
    console.warn('警告: grpc-tools 的 protoc 可能不支持 C# 输出');
    return grpcProtoc;
  }
  
  return null;
}

// 生成单个 proto 文件的 C# 代码
function generateCSharp(protocPath, protoFile, outputDir) {
  const protoPath = path.resolve(path.join(PROTO_DIR, protoFile));
  const resolvedProtoDir = path.resolve(PROTO_DIR);
  const resolvedOutputDir = path.resolve(outputDir);
  
  if (!fs.existsSync(protoPath)) {
    console.error(`Proto文件不存在: ${protoPath}`);
    return false;
  }

  try {
    // 构建命令（使用引号包裹路径以处理空格）
    const quotedProtoc = protocPath.includes(' ') || protocPath.includes('\\') ? `"${protocPath}"` : protocPath;
    const command = `${quotedProtoc} --proto_path="${resolvedProtoDir}" --csharp_out="${resolvedOutputDir}" "${protoPath}"`;
    
    console.log(`正在生成: ${protoFile}`);
    console.log(`命令: ${command}`);
    
    const result = execSync(command, {
      encoding: 'utf-8',
      stdio: ['pipe', 'pipe', 'pipe'],
      maxBuffer: 10 * 1024 * 1024 // 10MB buffer
    });
    
    if (result) {
      console.log(result);
    }
    console.log(`✓ 成功生成: ${protoFile}`);
    return true;
  } catch (error) {
    console.error(`生成失败: ${protoFile}`);
    
    // 尝试获取完整的错误信息
    const stdout = error.stdout ? error.stdout.toString().trim() : '';
    const stderr = error.stderr ? error.stderr.toString().trim() : '';
    
    if (stdout) {
      console.error('标准输出:', stdout);
    }
    if (stderr) {
      console.error('错误输出:', stderr);
    }
    
    if (!stdout && !stderr) {
      console.error('错误消息:', error.message);
      console.error('退出码:', error.status);
    }
    
    return false;
  }
}

// 生成所有 proto 文件
function generateAll() {
  const outputDir = getOutputDir();
  const resolvedProtoDir = path.resolve(PROTO_DIR);
  
  console.log('=== Proto to C# 代码生成器 ===\n');
  console.log(`Proto目录: ${resolvedProtoDir}`);
  console.log(`输出目录: ${outputDir}\n`);

  // 查找 protoc
  const protocPath = findProtoc();
  if (!protocPath) {
    console.error('错误: 未找到 protoc 编译器\n');
    console.log('请安装官方 protoc 编译器:\n');
    console.log('  Windows:');
    console.log('    1. 下载: https://github.com/protocolbuffers/protobuf/releases');
    console.log('       选择 protoc-XX.X-win64.zip');
    console.log('    2. 解压到目录如 C:\\protoc');
    console.log('    3. 将 C:\\protoc\\bin 添加到系统 PATH 环境变量\n');
    console.log('  macOS: brew install protobuf');
    console.log('  Linux: apt install protobuf-compiler');
    return;
  }

  // 检查 proto 目录
  if (!fs.existsSync(resolvedProtoDir)) {
    console.error(`Proto目录不存在: ${resolvedProtoDir}`);
    return;
  }

  // 确保输出目录存在
  ensureDirectoryExists(outputDir);

  // 获取所有 proto 文件
  const protoFiles = fs.readdirSync(resolvedProtoDir)
    .filter(file => file.endsWith('.proto'));

  if (protoFiles.length === 0) {
    console.log('未找到 .proto 文件');
    return;
  }

  console.log(`找到 ${protoFiles.length} 个 proto 文件\n`);

  let successCount = 0;
  let failCount = 0;

  protoFiles.forEach(file => {
    if (generateCSharp(protocPath, file, outputDir)) {
      successCount++;
    } else {
      failCount++;
    }
  });

  console.log(`\n生成完成: 成功 ${successCount} 个, 失败 ${failCount} 个`);
  console.log(`输出目录: ${outputDir}`);
}

// 显示帮助
function showHelp() {
  console.log('Proto to C# 代码生成器');
  console.log('\n用法:');
  console.log('  node generate-proto-csharp.js [选项]');
  console.log('\n选项:');
  console.log('  -o, --output <path>  指定输出目录');
  console.log('  -h, --help           显示帮助信息');
  console.log('\n示例:');
  console.log('  node generate-proto-csharp.js');
  console.log('  node generate-proto-csharp.js -o ../Unity/Assets/Scripts/Proto');
  console.log('  npm run proto:csharp');
  console.log('  npm run proto:csharp -- -o ../Unity/Assets/Scripts/Proto');
}

// 主函数
function main() {
  const args = process.argv.slice(2);
  
  if (args.includes('-h') || args.includes('--help')) {
    showHelp();
    return;
  }

  generateAll();
}

main();
