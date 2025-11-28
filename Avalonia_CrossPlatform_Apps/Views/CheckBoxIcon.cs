using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Layout;

namespace Avalonia_CrossPlatform_Apps.Views;

/// <summary>
/// 自定义复选框图标控件，始终显示方框，选中时显示勾选标记
/// </summary>
public class CheckBoxIcon : UserControl
{
    private readonly Border _border;
    private readonly TextBlock _checkMark;
    
    public CheckBoxIcon()
    {
        // 创建方框（增大尺寸）
        _border = new Border
        {
            Width = 16,
            Height = 16,
            BorderBrush = new SolidColorBrush(Color.FromRgb(128, 128, 128)),
            BorderThickness = new Avalonia.Thickness(1),
            Background = Brushes.Transparent,
            CornerRadius = new Avalonia.CornerRadius(2)
        };
        
        // 使用 Unicode 字符显示勾选标记
        _checkMark = new TextBlock
        {
            Text = "✓",  // Unicode 勾选字符 (U+2713)
            FontSize = 12,
            Foreground = new SolidColorBrush(Colors.White),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            IsVisible = false
        };
        
        // 创建容器
        var container = new Grid
        {
            Width = 16,
            Height = 16,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center
        };
        
        container.Children.Add(_border);
        container.Children.Add(_checkMark);
        
        Content = container;
    }
    
    public bool IsChecked
    {
        get => _checkMark.IsVisible;
        set => _checkMark.IsVisible = value;
    }
}

