using System;
using System.Globalization;
using System.Windows.Data;

namespace WallpaperManager.Controls
{
    [ValueConversion(typeof(TimeSpan), typeof(string))]
    public class TimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((TimeSpan)value).ToString("dd' 'hh':'mm");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return TimeSpan.ParseExact(value.ToString() ?? string.Empty, "dd' 'hh':'mm", CultureInfo.CurrentCulture);
        }
    }
}
