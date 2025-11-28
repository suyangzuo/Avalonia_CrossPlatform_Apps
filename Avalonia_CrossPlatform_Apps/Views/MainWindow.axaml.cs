using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using System.ComponentModel;

namespace Avalonia_CrossPlatform_Apps.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        // 处理 Windows 平台的自定义标题栏
        // 当 ExtendClientAreaToDecorationsHint="True" 时，需要设置 Padding 以避免内容被标题栏遮挡
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // 监听窗口属性变化，更新 Padding
            this.PropertyChanged += (sender, e) =>
            {
                if (e.Property == ExtendClientAreaTitleBarHeightHintProperty)
                {
                    UpdateTitleBarPadding();
                }
            };
            
            // 初始化时设置 Padding
            Loaded += (sender, e) => UpdateTitleBarPadding();
        }
        
        // 初始化时钟画布的可见性，确保与菜单项的初始状态一致
        Loaded += (sender, e) => 
        {
            InitializeClockVisibility();
            InitializeCheckBoxIcon();
        };
    }
    
    private void InitializeCheckBoxIcon()
    {
        if (ShowClockMenuItem != null)
        {
            // 初始化选中状态为 true
            ShowClockMenuItem.Tag = true;
            
            // 创建自定义复选框图标
            var checkBoxIcon = new CheckBoxIcon
            {
                IsChecked = true
            };
            
            // 设置菜单项图标
            ShowClockMenuItem.Icon = checkBoxIcon;
        }
    }
    
    private void InitializeClockVisibility()
    {
        if (ShowClockMenuItem != null && MainContentView != null)
        {
            var clockCanvas = MainContentView.FindControl<Avalonia.Controls.Canvas>("ClockCanvas");
            if (clockCanvas != null)
            {
                // 根据菜单项的初始状态设置时钟画布的可见性（默认为 true）
                clockCanvas.IsVisible = ShowClockMenuItem.Tag as bool? ?? true;
            }
        }
    }
    
    private void UpdateTitleBarPadding()
    {
        // 获取系统标题栏高度
        var titleBarHeight = this.ExtendClientAreaTitleBarHeightHint;
        if (titleBarHeight <= 0)
        {
            // 如果无法获取，使用默认值（通常是 32-40 像素）
            titleBarHeight = 32;
        }
        
        // 设置 Padding，确保内容不被标题栏遮挡
        // 只在上方添加 padding，左右和下方保持原样
        Padding = new Avalonia.Thickness(0, titleBarHeight, 0, 0);
    }
    
    // 退出菜单项点击事件
    private void ExitMenuItem_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close();
    }
    
    // 显示时钟菜单项点击事件
    private void ShowClockMenuItem_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (sender is MenuItem menuItem && MainContentView != null)
        {
            // 切换选中状态
            bool newCheckedState = !(menuItem.Tag as bool? ?? false);
            menuItem.Tag = newCheckedState;
            
            // 更新图标状态
            if (menuItem.Icon is CheckBoxIcon icon)
            {
                icon.IsChecked = newCheckedState;
            }
            
            // 根据复选框状态显示或隐藏时钟表盘（只隐藏时钟，日期和时间继续显示）
            var clockCanvas = MainContentView.FindControl<Avalonia.Controls.Canvas>("ClockCanvas");
            if (clockCanvas != null)
            {
                clockCanvas.IsVisible = newCheckedState;
            }
        }
    }
    
    // 关于菜单项点击事件
    private void AboutMenuItem_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        // 创建一个新的窗口作为对话框
        var aboutWindow = new Window
        {
            Title = "应用信息",
            Width = 400,
            Height = 250,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false,
            ShowInTaskbar = false
        };
        
        var okButton = new Button
        {
            Content = "确定",
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center,
            Margin = new Avalonia.Thickness(0, 20, 0, 0),
            Width = 100
        };
        okButton.Click += (s, args) => aboutWindow.Close();
        
        aboutWindow.Content = new StackPanel
        {
            Margin = new Avalonia.Thickness(20),
            Spacing = 10,
            Children =
            {
                new TextBlock
                {
                    Text = "时钟应用",
                    FontSize = 20,
                    FontWeight = Avalonia.Media.FontWeight.Bold,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                    Margin = new Avalonia.Thickness(0, 0, 0, 10)
                },
                new TextBlock
                {
                    Text = "版本: 1.0.0",
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
                },
                new TextBlock
                {
                    Text = "基于 Avalonia UI 框架开发",
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
                },
                new TextBlock
                {
                    Text = "支持 Windows、Linux 和 macOS 平台",
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
                },
                okButton
            }
        };
        
        aboutWindow.ShowDialog(this);
    }
}