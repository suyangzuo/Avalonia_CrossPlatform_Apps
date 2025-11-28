# ⏰ Avalonia 跨平台时钟应用

一个基于 Avalonia UI 框架开发的精美跨平台时钟应用，支持 Windows、macOS 和 Linux 系统。

## 📋 功能特点

- 🕐 实时模拟时钟显示，平滑的指针动画
- 📅 数字日期和时钟显示
- 🎨 现代化深色主题界面
- 🌐 跨平台支持 (Windows, macOS, Linux)
- ⚡ 高精度计时，每100毫秒更新一次

## 🚀 快速开始

### 前提条件

- .NET 6.0 或更高版本
- 适用于您的操作系统的 Avalonia UI SDK

### 安装与运行

1. 克隆仓库
   ```bash
   git clone https://github.com/yourusername/Avalonia_CrossPlatform_Clock.git
   cd Avalonia_CrossPlatform_Clock
   ```

2. 还原依赖
   ```bash
   dotnet restore
   ```

3. 运行应用
   ```bash
   dotnet run --project Avalonia_CrossPlatform_Apps.Desktop
   ```

## 📁 项目结构

```
Avalonia_CrossPlatform_Clock/
├── Avalonia_CrossPlatform_Apps/          # 主要应用代码
│   ├── Assets/                           # 资源文件
│   ├── ViewModels/                       # 视图模型
│   └── Views/                            # 用户界面
├── Avalonia_CrossPlatform_Apps.Desktop/ # 桌面平台特定代码
└── README.md
```

## 🛠️ 技术栈

- **框架**: Avalonia UI
- **语言**: C#
- **架构**: MVVM (Model-View-ViewModel)

## 📝 开发说明

本项目采用 MVVM 架构模式，主要组件包括：

- **MainViewModel**: 处理时间数据和时钟逻辑
- **MainView**: 时钟界面的 XAML 定义
- **MainWindow**: 应用主窗口

时钟指针每100毫秒更新一次，确保平滑移动和精确显示。

## 🤝 贡献

欢迎提交 Pull Request 或 Issue 来改进这个项目！

## 📄 许可证

本项目采用 MIT 许可证 - 查看 [LICENSE](LICENSE) 文件了解详情。

## 🙏 致谢

感谢 [Avalonia UI](https://avaloniaui.net/) 提供强大的跨平台 UI 框架。
