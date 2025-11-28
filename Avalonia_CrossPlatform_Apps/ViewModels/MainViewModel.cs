using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Timers;

namespace Avalonia_CrossPlatform_Apps.ViewModels
{
    public partial class MainViewModel : INotifyPropertyChanged
    {
        // 日期和时间属性
        private string _currentDate = string.Empty;
        public string CurrentDate
        {
            get => _currentDate;
            set => SetField(ref _currentDate, value);
        }

        private string _currentTime = string.Empty;
        public string CurrentTime
        {
            get => _currentTime;
            set => SetField(ref _currentTime, value);
        }

        // 时钟指针角度
        private double _hourAngle = 0;
        public double HourAngle
        {
            get => _hourAngle;
            set => SetField(ref _hourAngle, value);
        }

        private double _minuteAngle = 0;
        public double MinuteAngle
        {
            get => _minuteAngle;
            set => SetField(ref _minuteAngle, value);
        }

        private double _secondAngle = 0;
        public double SecondAngle
        {
            get => _secondAngle;
            set => SetField(ref _secondAngle, value);
        }

        private readonly Timer _timer;
        private int _lastSecond = -1; // 记录上一次的秒数，用于检测秒数变化
        private DateTime _lastDate = DateTime.MinValue; // 记录上一次的日期，用于检测日期变化

        public MainViewModel()
        {
            // 初始化时间
            UpdateDateTime();

            // 设置计时器，每100ms更新一次，提高精度和指针平滑度
            _timer = new Timer(100);
            _timer.Elapsed += (sender, e) => UpdateDateTime();
            _timer.Start();
        }

        private void UpdateDateTime()
        {
            var now = DateTime.Now;

            // 检测日期是否变化（跨天时）
            if (_lastDate.Date != now.Date)
            {
                CurrentDate = now.ToString("yyyy年MM月dd日 dddd");
                _lastDate = now.Date;
            }

            // 检测秒数是否变化，只在变化时更新时间字符串（提高精度）
            if (_lastSecond != now.Second)
            {
                CurrentTime = now.ToString("HH:mm:ss");
                _lastSecond = now.Second;
            }

            // 计算时钟指针角度（每100ms更新一次，保持指针平滑移动）
            // 秒针：每秒6度（360/60），加上毫秒的影响
            SecondAngle = now.Second * 6 + now.Millisecond * 0.006;

            // 分针：每分钟6度，加上秒针和毫秒的影响
            MinuteAngle = now.Minute * 6 + now.Second * 0.1 + now.Millisecond * 0.0001;

            // 时针：每小时30度（360/12），加上分钟、秒和毫秒的影响
            HourAngle = (now.Hour % 12) * 30 + now.Minute * 0.5 + now.Second * (0.5 / 60) + now.Millisecond * (0.5 / 60000);
        }

        // INotifyPropertyChanged 实现
        public event PropertyChangedEventHandler? PropertyChanged;

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
