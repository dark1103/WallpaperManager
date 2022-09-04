using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WallpaperManager.Data
{
    public class TimeSpanValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return TimeSpan.TryParseExact(value.ToString() ?? string.Empty, "dd' 'hh':'mm", CultureInfo.CurrentCulture, out var _) ? ValidationResult.ValidResult : new ValidationResult(false, "Invalid");
        }
    }
}
