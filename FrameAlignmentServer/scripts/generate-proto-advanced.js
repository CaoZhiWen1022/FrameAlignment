const fs = require('fs');
const path = require('path');
const protobuf = require('protobufjs');

// 配置
const PROTO_DIR = path.join(__dirname, '../../Proto');
const OUTPUT_DIR = path.join(__dirname, '../src/generated');
const INTERFACE_OUTPUT_DIR = path.join(__dirname, '../src/generated/interfaces');

// 确保目录存在
function ensureDirectoryExists(dirPath) {
  if (!fs.existsSync(dirPath)) {
    fs.mkdirSync(dirPath, { recursive: true });
  }
}

// 将proto类型转换为TypeScript类型
function protoTypeToTS(protoType, repeated = false) {
  const typeMap = {
    'double': 'number',
    'float': 'number',
    'int32': 'number',
    'int64': 'number',
    'uint32': 'number',
    'uint64': 'number',
    'sint32': 'number',
    'sint64': 'number',
    'fixed32': 'number',
    'fixed64': 'number',
    'sfixed32': 'number',
    'sfixed64': 'number',
    'bool': 'boolean',
    'string': 'string',
    'bytes': 'Uint8Array',
  };

  const tsType = typeMap[protoType] || protoType;
  return repeated ? `${tsType}[]` : tsType;
}

// 生成接口定义
function generateInterface(messageType) {
  const fields = [];
  
  messageType.fieldsArray.forEach(field => {
    const tsType = protoTypeToTS(field.type, field.repeated);
    const optional = field.optional ? '?' : '';
    const comment = field.comment ? ` // ${field.comment}` : '';
    fields.push(`  ${field.name}${optional}: ${tsType};${comment}`);
  });

  return `export interface ${messageType.name} {\n${fields.join('\n')}\n}`;
}

// 生成枚举定义
function generateEnum(enumType) {
  const values = [];
  
  enumType.valuesArray.forEach(value => {
    const comment = value.comment ? ` // ${value.comment}` : '';
    values.push(`  ${value.name} = ${value.number},${comment}`);
  });

  return `export enum ${enumType.name} {\n${values.join('\n')}\n}`;
}

// 处理proto文件
async function processProtoFile(protoFile) {
  const protoPath = path.join(PROTO_DIR, protoFile);
  
  if (!fs.existsSync(protoPath)) {
    console.error(`Proto文件不存在: ${protoPath}`);
    return null;
  }

  try {
    const root = await protobuf.load(protoPath);
    const definitions = [];

    // 处理枚举
    root.nestedArray.forEach(nested => {
      if (nested instanceof protobuf.Enum) {
        definitions.push(generateEnum(nested));
      }
    });

    // 处理消息类型
    root.nestedArray.forEach(nested => {
      if (nested instanceof protobuf.Type) {
        definitions.push(generateInterface(nested));
      }
    });

    // 处理嵌套命名空间
    root.nestedArray.forEach(nested => {
      if (nested instanceof protobuf.Namespace) {
        nested.nestedArray.forEach(subNested => {
          if (subNested instanceof protobuf.Enum) {
            definitions.push(generateEnum(subNested));
          } else if (subNested instanceof protobuf.Type) {
            definitions.push(generateInterface(subNested));
          }
        });
      }
    });

    if (definitions.length === 0) {
      console.warn(`未找到类型定义: ${protoFile}`);
      return null;
    }

    const fileName = path.basename(protoFile, '.proto');
    const outputPath = path.join(INTERFACE_OUTPUT_DIR, `${fileName}.ts`);
    const content = `// 此文件由脚本自动生成，请勿手动修改\n// 来源: ${protoFile}\n\n${definitions.join('\n\n')}\n`;
    
    fs.writeFileSync(outputPath, content);
    console.log(`✓ 生成接口定义: ${outputPath}`);
    
    return fileName;
  } catch (error) {
    console.error(`处理失败: ${protoFile}`, error.message);
    return null;
  }
}

// 生成索引文件
function generateIndexFile(fileNames) {
  const indexContent = fileNames
    .filter(name => name !== null)
    .map(name => `export * from './interfaces/${name}';`)
    .join('\n');

  const indexPath = path.join(OUTPUT_DIR, 'index.ts');
  fs.writeFileSync(indexPath, `// 自动生成的接口定义索引\n\n${indexContent}\n`);
  console.log(`✓ 生成索引文件: ${indexPath}`);
}

// 主函数
async function main() {
  console.log('开始生成TypeScript接口定义...\n');
  
  ensureDirectoryExists(OUTPUT_DIR);
  ensureDirectoryExists(INTERFACE_OUTPUT_DIR);

  if (!fs.existsSync(PROTO_DIR)) {
    console.error(`Proto目录不存在: ${PROTO_DIR}`);
    console.log(`请确保 Proto 目录存在于项目根目录的上级目录中`);
    return;
  }

  const protoFiles = fs.readdirSync(PROTO_DIR)
    .filter(file => file.endsWith('.proto'));

  if (protoFiles.length === 0) {
    console.log('未找到.proto文件');
    return;
  }

  const results = [];
  for (const file of protoFiles) {
    const result = await processProtoFile(file);
    results.push(result);
  }

  generateIndexFile(results);
  console.log(`\n生成完成: 共处理 ${protoFiles.length} 个文件`);
}

main().catch(console.error);

