using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Controls.Documents;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Avalonia_CrossPlatform_Apps.ViewModels;
using Avalonia.Layout;

namespace Avalonia_CrossPlatform_Apps.Views;

public partial class MainView : UserControl
{
    private const double ClockRadius = 150.0; // 时钟半径
    private const double ClockCenterX = 150.0;
    private const double ClockCenterY = 150.0;
    private const double HourMarkLength = 15.0; // 小时刻度长度
    private const double HourMarkRadius = 135.0; // 小时刻度位置半径
    private const double MinuteMarkLength = 8.0; // 分钟刻度长度
    private const double MinuteMarkRadius = 140.0; // 分钟刻度位置半径
    private const double NumberRadius = 115.0; // 数字位置半径（比刻度更靠里）
    private const double HourMarkThickness = 3.0;
    private const double MinuteMarkThickness = 2.0;

    // 保存对 ViewModel 的引用，用于移除事件监听
    private MainViewModel? _currentViewModel;

    // 颜色定义
    private static readonly IBrush DateLabelColor = new SolidColorBrush(Color.FromRgb(150, 150, 150)); // 年、月、日文字颜色（灰色）
    private static readonly IBrush DateNumberColor = Brushes.White; // 日期数字颜色（白色）
    private static readonly IBrush WeekdayColor = new SolidColorBrush(Color.FromRgb(100, 200, 255)); // 周几颜色（浅蓝色）
    private static readonly IBrush TimeNumberColor = Brushes.White; // 时间数字颜色（白色）
    private static readonly IBrush TimeColonColor = new SolidColorBrush(Color.FromRgb(150, 150, 150)); // 冒号颜色（浅灰色）
    
    // 日期标签的左右边距
    private const double DateLabelMargin = 4.0; // 可以根据需要调整

    public MainView()
    {
        InitializeComponent();
        Loaded += MainView_Loaded;
        DataContextChanged += MainView_DataContextChanged;
    }

    private void MainView_DataContextChanged(object? sender, EventArgs e)
    {
        // 移除之前的监听（如果有）
        if (_currentViewModel != null)
        {
            _currentViewModel.PropertyChanged -= ViewModel_PropertyChanged;
            _currentViewModel = null;
        }
        
        if (DataContext is MainViewModel viewModel)
        {
            _currentViewModel = viewModel;
            // 监听属性变化
            viewModel.PropertyChanged += ViewModel_PropertyChanged;
            // 初始化显示
            UpdateDateDisplay(viewModel.CurrentDate);
            UpdateTimeDisplay(viewModel.CurrentTime);
        }
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is MainViewModel viewModel)
        {
            // 确保在 UI 线程上执行
            if (Dispatcher.UIThread.CheckAccess())
            {
                UpdateUI(viewModel, e.PropertyName);
            }
            else
            {
                Dispatcher.UIThread.Post(() => UpdateUI(viewModel, e.PropertyName));
            }
        }
    }

    private void UpdateUI(MainViewModel viewModel, string? propertyName)
    {
        if (propertyName == nameof(MainViewModel.CurrentDate))
        {
            UpdateDateDisplay(viewModel.CurrentDate);
        }
        else if (propertyName == nameof(MainViewModel.CurrentTime))
        {
            UpdateTimeDisplay(viewModel.CurrentTime);
        }
    }

    private void UpdateDateDisplay(string dateString)
    {
        var dateTextBlock = this.FindControl<TextBlock>("DateTextBlock");
        if (dateTextBlock == null)
        {
            // 如果找不到控件，尝试在下一个 UI 周期重试
            Dispatcher.UIThread.Post(() =>
            {
                var retryBlock = this.FindControl<TextBlock>("DateTextBlock");
                if (retryBlock != null)
                {
                    UpdateDateDisplayInternal(retryBlock, dateString);
                }
            });
            return;
        }
        UpdateDateDisplayInternal(dateTextBlock, dateString);
    }

    private void UpdateDateDisplayInternal(TextBlock dateTextBlock, string dateString)
    {

        dateTextBlock.Inlines?.Clear();
        if (dateTextBlock.Inlines == null) return;

        // 解析日期字符串，格式：yyyy年MM月dd日 dddd
        // 例如：2024年01月15日 星期一
        var parts = dateString.Split(' ');
        if (parts.Length >= 2)
        {
            string datePart = parts[0]; // yyyy年MM月dd日
            string weekdayPart = parts[1]; // dddd

            // 解析日期部分
            int yearEnd = datePart.IndexOf('年');
            int monthEnd = datePart.IndexOf('月');
            int dayEnd = datePart.IndexOf('日');

            if (yearEnd > 0 && monthEnd > yearEnd && dayEnd > monthEnd)
            {
                string year = datePart.Substring(0, yearEnd);
                string month = datePart.Substring(yearEnd + 1, monthEnd - yearEnd - 1);
                string day = datePart.Substring(monthEnd + 1, dayEnd - monthEnd - 1);

                // 获取父 TextBlock 的字体属性
                var fontFamily = dateTextBlock.FontFamily;
                var fontSize = dateTextBlock.FontSize;

                // 年
                dateTextBlock.Inlines.Add(new Run(year) { Foreground = DateNumberColor });
                dateTextBlock.Inlines.Add(CreateLabelWithMargin("年", fontFamily, fontSize));
                
                // 月
                dateTextBlock.Inlines.Add(new Run(month) { Foreground = DateNumberColor });
                dateTextBlock.Inlines.Add(CreateLabelWithMargin("月", fontFamily, fontSize));
                
                // 日
                dateTextBlock.Inlines.Add(new Run(day) { Foreground = DateNumberColor });
                dateTextBlock.Inlines.Add(CreateLabelWithMargin("日", fontFamily, fontSize));
                
                // 空格
                dateTextBlock.Inlines.Add(new Run(" ") { Foreground = DateLabelColor });
                
                // 周几
                dateTextBlock.Inlines.Add(new Run(weekdayPart) { Foreground = WeekdayColor });
            }
        }
    }

    // 创建带 Margin 的标签（年、月、日）
    private InlineUIContainer CreateLabelWithMargin(string label, FontFamily fontFamily, double fontSize)
    {
        // 使用 RenderTransform 来精确调整垂直位置
        // TextBlock 在 InlineUIContainer 中默认会偏上，需要向下移动
        // 这个偏移量基于字体大小，可能需要根据实际效果微调
        double verticalOffset = fontSize * 0.3; // 增加偏移量
        
        var labelTextBlock = new TextBlock
        {
            Text = label,
            Foreground = DateLabelColor,
            FontFamily = fontFamily,
            FontSize = fontSize,
            Margin = new Avalonia.Thickness(DateLabelMargin, 0, DateLabelMargin, 0),
            VerticalAlignment = VerticalAlignment.Top,
            RenderTransform = new TranslateTransform { Y = verticalOffset }
        };
        
        return new InlineUIContainer
        {
            Child = labelTextBlock
        };
    }

    private void UpdateTimeDisplay(string timeString)
    {
        var timeTextBlock = this.FindControl<TextBlock>("TimeTextBlock");
        if (timeTextBlock == null)
        {
            // 如果找不到控件，尝试在下一个 UI 周期重试
            Dispatcher.UIThread.Post(() =>
            {
                var retryBlock = this.FindControl<TextBlock>("TimeTextBlock");
                if (retryBlock != null)
                {
                    UpdateTimeDisplayInternal(retryBlock, timeString);
                }
            });
            return;
        }
        UpdateTimeDisplayInternal(timeTextBlock, timeString);
    }

    private void UpdateTimeDisplayInternal(TextBlock timeTextBlock, string timeString)
    {

        timeTextBlock.Inlines?.Clear();
        if (timeTextBlock.Inlines == null) return;

        // 解析时间字符串，格式：HH:mm:ss
        // 例如：14:30:45
        var parts = timeString.Split(':');
        if (parts.Length == 3)
        {
            // 小时
            timeTextBlock.Inlines.Add(new Run(parts[0]) { Foreground = TimeNumberColor });
            timeTextBlock.Inlines.Add(new Run(":") { Foreground = TimeColonColor });
            
            // 分钟
            timeTextBlock.Inlines.Add(new Run(parts[1]) { Foreground = TimeNumberColor });
            timeTextBlock.Inlines.Add(new Run(":") { Foreground = TimeColonColor });
            
            // 秒
            timeTextBlock.Inlines.Add(new Run(parts[2]) { Foreground = TimeNumberColor });
        }
    }

    private void MainView_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        DrawClockMarks();
        
        // 如果 DataContext 为空，尝试从父窗口获取
        if (DataContext == null)
        {
            var parent = this.Parent;
            while (parent != null && DataContext == null)
            {
                DataContext = parent.DataContext;
                parent = parent.Parent;
            }
        }
        
        // 初始化日期和时间显示，并设置属性监听
        // 确保即使 DataContextChanged 没有触发，也能正常工作
        if (DataContext is MainViewModel viewModel)
        {
            // 更新当前 ViewModel 引用
            if (_currentViewModel != viewModel)
            {
                // 移除之前的监听（如果有）
                if (_currentViewModel != null)
                {
                    _currentViewModel.PropertyChanged -= ViewModel_PropertyChanged;
                }
                _currentViewModel = viewModel;
            }
            
            // 移除之前的监听（避免重复）
            viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            // 添加属性监听
            viewModel.PropertyChanged += ViewModel_PropertyChanged;
            
            // 初始化显示
            UpdateDateDisplay(viewModel.CurrentDate);
            UpdateTimeDisplay(viewModel.CurrentTime);
        }
    }

    private void DrawClockMarks()
    {
        var canvas = this.FindControl<Canvas>("ClockCanvas");
        if (canvas == null) return;

        // 清除现有的刻度和数字（保留背景圆和指针）
        // 指针是在 Grid 内部的，不会被移除
        var toRemove = new List<Control>();
        foreach (var child in canvas.Children)
        {
            // 移除刻度线 Path 和数字 TextBlock
            if (child is Path path && path.Name?.StartsWith("ClockMark") == true)
            {
                toRemove.Add(path);
            }
            else if (child is TextBlock textBlock && textBlock.Name?.StartsWith("ClockNumber") == true)
            {
                toRemove.Add(textBlock);
            }
        }
        foreach (var item in toRemove)
        {
            canvas.Children.Remove(item);
        }

        // 创建字体属性
        var fontFamily = new FontFamily("Google Sans Code");
        var fontSize = 16.0;
        var fontWeight = FontWeight.Bold;

        // 绘制60个分钟刻度（跳过小时位置，即每5分钟的位置）
        for (int i = 0; i < 60; i++)
        {
            // 跳过小时位置（每5分钟一个，即 i % 5 == 0 的位置）
            if (i % 5 == 0) continue;

            double angle = i * 6.0 - 90.0; // 从12点开始，逆时针
            double angleRad = angle * Math.PI / 180.0;

            // 计算刻度的起点和终点（使用圆形坐标）
            double startX = ClockCenterX + MinuteMarkRadius * Math.Cos(angleRad);
            double startY = ClockCenterY + MinuteMarkRadius * Math.Sin(angleRad);
            double endX = ClockCenterX + (MinuteMarkRadius + MinuteMarkLength) * Math.Cos(angleRad);
            double endY = ClockCenterY + (MinuteMarkRadius + MinuteMarkLength) * Math.Sin(angleRad);

            var path = new Path
            {
                Name = $"ClockMarkMinute{i}",
                Data = new LineGeometry
                {
                    StartPoint = new Avalonia.Point(startX, startY),
                    EndPoint = new Avalonia.Point(endX, endY)
                },
                Stroke = new SolidColorBrush(Color.FromRgb(100, 100, 100)), // 分钟刻度颜色
                StrokeThickness = MinuteMarkThickness
            };

            canvas.Children.Add(path);
        }

        // 绘制12个小时刻度和数字
        for (int hour = 1; hour <= 12; hour++)
        {
            double angle = (hour * 30.0) - 90.0; // 从12点开始，逆时针
            double angleRad = angle * Math.PI / 180.0;

            // 绘制小时刻度（使用圆形坐标）
            double startX = ClockCenterX + HourMarkRadius * Math.Cos(angleRad);
            double startY = ClockCenterY + HourMarkRadius * Math.Sin(angleRad);
            double endX = ClockCenterX + (HourMarkRadius + HourMarkLength) * Math.Cos(angleRad);
            double endY = ClockCenterY + (HourMarkRadius + HourMarkLength) * Math.Sin(angleRad);

            var hourPath = new Path
            {
                Name = $"ClockMarkHour{hour}",
                Data = new LineGeometry
                {
                    StartPoint = new Avalonia.Point(startX, startY),
                    EndPoint = new Avalonia.Point(endX, endY)
                },
                Stroke = new SolidColorBrush(Color.FromRgb(128, 128, 128)), // 小时刻度颜色
                StrokeThickness = HourMarkThickness
            };

            canvas.Children.Add(hourPath);

            // 绘制数字
            string hourText = hour.ToString();
            var textBlock = new TextBlock
            {
                Text = hourText,
                FontFamily = fontFamily,
                FontSize = fontSize,
                FontWeight = fontWeight,
                Foreground = Brushes.LightCyan, // 时钟数字颜色
                Name = $"ClockNumber{hour}"
            };

            // 使用 Measure 方法精确测量文本尺寸
            textBlock.Measure(Avalonia.Size.Infinity);
            var textSize = textBlock.DesiredSize;
            double textWidth = textSize.Width;
            double textHeight = textSize.Height;

            // 计算数字位置（使用圆形坐标，并减去文本尺寸的一半以居中）
            double numberX = ClockCenterX + NumberRadius * Math.Cos(angleRad) - textWidth / 2.0;
            double numberY = ClockCenterY + NumberRadius * Math.Sin(angleRad) - textHeight / 2.0;

            Canvas.SetLeft(textBlock, numberX);
            Canvas.SetTop(textBlock, numberY);

            canvas.Children.Add(textBlock);
        }
    }
}