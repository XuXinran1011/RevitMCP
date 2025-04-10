# RevitMCP 安装指南

本指南提供了安装和设置RevitMCP的详细说明，以实现与Autodesk Revit的自然语言交互。

## 前提条件

在安装RevitMCP之前，请确保您具备以下前提条件：

- **操作系统**：Windows 10或更高版本
- **Autodesk Revit**：已安装并获得许可的Revit 2025
- **.NET框架**：已安装.NET 8或.NET 9
- **MCP客户端**：Claude Desktop或其他兼容MCP的客户端
- **开发工具**（针对开发人员）：
  - Visual Studio 2022或更高版本（推荐）
  - Git

## 安装选项

RevitMCP可以通过以下方法之一进行安装：

1. [NuGet包安装](#nuget包安装)（推荐给大多数用户）
2. [手动安装](#手动安装)
3. [从源代码构建](#从源代码构建)（适用于开发人员）

## NuGet包安装

### 步骤1：安装RevitMCP NuGet包

1. 在Visual Studio中打开您的.NET项目
2. 在解决方案资源管理器中右键点击您的项目，选择"管理NuGet包"
3. 点击"浏览"选项卡
4. 搜索"RevitMCP"
5. 选择包并点击"安装"

或者，您可以使用Package Manager Console安装包：

```powershell
Install-Package RevitMCP
```

或使用.NET CLI：

```bash
dotnet add package RevitMCP
```

### 步骤2：配置RevitMCP

1. 在项目根目录中创建名为`revitmcp.config.json`的配置文件：

```json
{
  "revit": {
    "connectionMode": "Active",
    "timeout": 30
  },
  "mcp": {
    "serverName": "RevitMCP",
    "serverVersion": "1.0.0",
    "transport": "stdio"
  },
  "logging": {
    "level": "Information",
    "outputPath": "logs/revitmcp.log"
  }
}
```

2. 将配置文件添加到您的项目中，并设置为复制到输出目录

## 手动安装

### 步骤1：下载RevitMCP包

1. 从[GitHub发布页面](https://github.com/XuXinran1011/RevitMCP/releases)下载最新的RevitMCP发布包
2. 将包解压到您计算机上的目录

### 步骤2：为Revit安装RevitMCP插件

1. 导航到解压后的包目录
2. 运行`RevitMCP_Setup.exe`安装程序
3. 按照屏幕上的说明完成安装
4. 如果Revit当前正在运行，请重启Revit

### 步骤3：验证安装

1. 打开Revit
2. 转到"加载项"选项卡
3. 查找"RevitMCP"面板
4. 点击"启动服务器"以启动RevitMCP服务器

## 从源代码构建

### 步骤1：克隆仓库

```bash
git clone https://github.com/XuXinran1011/RevitMCP.git
cd RevitMCP
```

### 步骤2：构建解决方案

使用Visual Studio：

1. 在Visual Studio中打开`RevitMCP.sln`解决方案文件
2. 选择所需的构建配置（Debug/Release）
3. 构建解决方案（Ctrl+Shift+B）

使用.NET CLI：

```bash
dotnet restore
dotnet build
```

### 步骤3：安装Revit插件

1. 导航到`build/addin`目录
2. 将`RevitMCP.addin`文件复制到Revit插件目录：
   - `%APPDATA%\Autodesk\Revit\Addins\2025\`
3. 将构建输出复制到`.addin`文件中指定的目录

## 配置Claude Desktop

要将RevitMCP与Claude Desktop一起使用：

### 步骤1：安装Claude Desktop

1. 如果您尚未安装，请下载并安装[Claude Desktop](https://claude.ai/desktop)
2. 启动Claude Desktop并登录您的账户

### 步骤2：配置MCP服务器

1. 打开Claude Desktop
2. 转到设置（齿轮图标）
3. 导航到"Model Context Protocol"部分
4. 点击"添加服务器"
5. 配置RevitMCP服务器：
   - **名称**：RevitMCP
   - **命令**：RevitMCP可执行文件的路径
   - **参数**：任何必需的命令行参数
   - **环境变量**：任何必需的环境变量

示例配置：

```json
"mcpServers": {
  "revitmcp": {
    "command": "C:\\Program Files\\RevitMCP\\RevitMCP.exe",
    "args": ["--transport", "stdio"],
    "env": {
      "REVIT_CONNECTION_MODE": "Active"
    }
  }
}
```

### 步骤3：测试连接

1. 在Claude Desktop中，开始一个新的对话
2. 输入使用RevitMCP的命令，例如"列出当前Revit模型中的所有墙"
3. Claude应该连接到RevitMCP并执行命令

## 故障排除

### 常见问题

#### Revit连接问题

**问题**：RevitMCP无法连接到Revit
**解决方案**：

- 确保Revit正在运行且已打开项目
- 检查您是否有正确版本的Revit（2025）
- 验证RevitMCP插件是否正确安装
- 检查RevitMCP日志以获取特定错误消息

#### MCP客户端连接问题

**问题**：Claude Desktop无法连接到RevitMCP
**解决方案**：

- 验证Claude Desktop配置中RevitMCP可执行文件的路径
- 确保RevitMCP服务器正在运行
- 检查Claude Desktop日志中的连接错误
- 尝试重启Claude Desktop

#### 插件加载问题

**问题**：RevitMCP插件未在Revit中显示
**解决方案**：

- 验证`.addin`文件是否在正确的Revit插件目录中
- 检查`.addin`文件中的路径是否指向正确位置
- 查看Revit日志文件中的任何错误消息
- 尝试重新安装插件

### 日志记录

RevitMCP创建的日志文件可以帮助诊断问题：

- **RevitMCP服务器日志**：位于配置的日志目录中
- **Revit日志文件**：位于`%APPDATA%\Autodesk\Revit\Journals\`
- **Claude Desktop日志**：位于Claude Desktop应用程序数据目录中

## 更新RevitMCP

要将RevitMCP更新到较新版本：

1. 从[GitHub发布页面](https://github.com/XuXinran1011/RevitMCP/releases)下载最新版本
2. 关闭Revit和Claude Desktop
3. 运行新版本的安装程序
4. 重启Revit和Claude Desktop

对于NuGet包安装，使用NuGet包管理器或Package Manager Console更新包：

```powershell
Update-Package RevitMCP
```

## 卸载RevitMCP

要卸载RevitMCP：

1. 关闭Revit和Claude Desktop
2. 在Windows设置中转到"添加或删除程序"
3. 在已安装程序列表中找到"RevitMCP"
4. 点击"卸载"并按照屏幕上的说明操作
5. 从Revit插件目录中删除RevitMCP插件文件

## 获取帮助

如果您遇到本指南未涵盖的问题：

- 查看[GitHub问题页面](https://github.com/XuXinran1011/RevitMCP/issues)了解已知问题
- 提交一个包含问题详细信息的新问题
- 向社区寻求支持
