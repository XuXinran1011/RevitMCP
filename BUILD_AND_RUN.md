# RevitMCP 构建和运行指南

本文档提供了如何构建和运行RevitMCP项目的详细说明。

## 先决条件

- Windows 10或更高版本
- .NET 9 SDK
- .NET 8 SDK
- Visual Studio 2022或更高版本（推荐）
- Autodesk Revit 2025

## 项目结构

RevitMCP解决方案包含以下项目：

1. **RevitMCP.Plugin** (.NET 8)
   - Revit插件项目
   - 负责与Revit API交互
   - 启动和管理RevitMCP服务器进程

2. **RevitMCP.Server** (.NET 9)
   - MCP服务器项目
   - 实现MCP协议
   - 处理LLM客户端请求

3. **RevitMCP.Shared** (netstandard2.0)
   - 共享库项目
   - 包含共享的模型和接口
   - 用于插件和服务器之间的通信

## 构建项目

### 使用Visual Studio

1. 打开`RevitMCP.sln`解决方案文件
2. 选择"Release"配置和"x64"平台
3. 右键点击解决方案，选择"生成解决方案"

### 使用命令行

```powershell
# 还原NuGet包
dotnet restore

# 构建解决方案
dotnet build -c Release
```

## 运行测试

### 测试RevitMCP.Server

```powershell
# 运行服务器测试
dotnet run --project RevitMCP.Server/RevitMCP.Server.csproj -- --test
```

## 部署到Revit

1. 构建解决方案
2. 将以下文件复制到Revit的插件目录（例如`%APPDATA%\Autodesk\Revit\Addins\2025`）：
   - `RevitMCP.Plugin.dll`
   - `RevitMCP.Shared.dll`
   - `RevitMCP.Server.dll`（以及其依赖项）
3. 创建一个`RevitMCP.addin`文件，内容如下：

```xml
<?xml version="1.0" encoding="utf-8"?>
<RevitAddIns>
  <AddIn Type="Application">
    <Name>RevitMCP</Name>
    <Assembly>RevitMCP.Plugin.dll</Assembly>
    <AddInId>8D7E5B3A-9F7A-4F1B-9A7C-7D5B7E5B3A9F</AddInId>
    <FullClassName>RevitMCP.Plugin.RevitMCPApp</FullClassName>
    <VendorId>XUXR</VendorId>
    <VendorDescription>Xu Xinran</VendorDescription>
  </AddIn>
</RevitAddIns>
```

## 使用方法

1. 启动Revit 2025
2. RevitMCP将自动加载，并在功能区中添加"RevitMCP"选项卡
3. 点击"启动MCP"按钮启动MCP服务器
4. 使用支持MCP的LLM客户端（如Claude Desktop）连接到服务器
5. 完成后，点击"停止MCP"按钮停止服务器

## 故障排除

### 常见问题

1. **找不到RevitMCP选项卡**
   - 确保插件已正确安装
   - 检查Revit日志文件中的错误

2. **无法启动MCP服务器**
   - 确保.NET 9运行时已安装
   - 检查服务器日志文件中的错误

3. **LLM客户端无法连接到服务器**
   - 确保服务器正在运行
   - 检查防火墙设置
   - 验证连接设置

### 日志文件

- Revit插件日志：`%APPDATA%\Autodesk\Revit\Logs`
- MCP服务器日志：`%TEMP%\RevitMCP\Logs`

## 开发注意事项

- Revit 2025最高支持.NET 8，因此RevitMCP.Plugin项目必须使用.NET 8
- MCP SDK需要.NET 9，因此RevitMCP.Server项目使用.NET 9
- 两个项目通过进程间通信进行交互，使用MCP的标准化消息格式和协议
