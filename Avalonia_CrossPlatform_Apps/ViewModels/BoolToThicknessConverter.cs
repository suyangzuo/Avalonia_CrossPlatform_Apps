using System;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Avalonia_CrossPlatform_Apps.ViewModels
{
    // 布尔值到线宽转换器
    public class BoolToThicknessConverter : IValueConverter
    {
        public static readonly BoolToThicknessConverter Instance = new BoolToThicknessConverter();

        public object Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool isHourMark)
            {
                return isHourMark ? 3 : 2; // 主刻度线宽为3，次要刻度线宽为2
            }
            return 2; // 默认线宽
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
