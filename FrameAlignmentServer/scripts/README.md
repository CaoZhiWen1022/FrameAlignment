# Proto 接口生成脚本

这些脚本用于从 Protocol Buffers (proto3) 文件生成 TypeScript 静态模块。

## 脚本说明

### generate-proto.js

使用 `protobufjs-cli` 工具生成完全自包含的 TypeScript 静态模块。

**特点：**
- ✅ **完全自包含**：生成的代码包含所有编解码逻辑，无需运行时依赖
- ✅ **静态编译**：性能更好，无需动态加载 proto 文件
- ✅ **类型安全**：自动生成 TypeScript 类型定义
- ✅ **可独立分发**：生成的代码可以直接复制到其他项目使用

**使用方法：**
```bash
npm run proto:generate
```

**监听模式：**
```bash
npm run proto:watch
```

## 目录结构

```
../../Proto/              # 存放 .proto 文件（在项目外部）
src/generated/            # 生成的 TypeScript 文件
  ├── proto/
  │   ├── proto.js        # 生成的 JavaScript 模块（包含编解码逻辑）
  │   ├── proto.d.ts      # TypeScript 类型定义
  │   └── index.ts        # 导出索引
  └── index.ts            # 主导出索引
```

## 使用示例

```typescript
import { proto } from './generated';

// 创建消息
const request = proto.RegisterRequest.create({
  base: { id: proto.ApiId.API_REGISTER_REQ },
  username: 'test',
  password: '123456',
  nickname: 'Test User',
});

// 编码为二进制
const buffer = proto.RegisterRequest.encode(request).finish();

// 解码二进制数据
const decoded = proto.RegisterRequest.decode(buffer);
console.log(decoded.username); // 'test'

// 使用枚举
if (decoded.base?.id === proto.ApiId.API_REGISTER_REQ) {
  console.log('这是注册请求');
}
```

## 注意事项

- 生成的文件会自动添加到 `.gitignore`，不应提交到版本控制
- 字段名使用 snake_case（如 `user_info`），与 proto 文件保持一致
- 所有类型都在 `proto` 命名空间下
