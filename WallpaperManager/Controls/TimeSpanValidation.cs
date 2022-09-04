using System;
using System.Globalization;
using System.Windows.Controls;

namespace WallpaperManager.Controls
{
    public class TimeSpanValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return TimeSpan.TryParseExact(value.ToString() ?? string.Empty, "dd' 'hh':'mm", CultureInfo.CurrentCulture, out var _) ? ValidationResult.ValidResult : new ValidationResult(false, "Invalid");
        }
    }
}
