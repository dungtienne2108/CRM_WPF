using System.Globalization;
using System.Windows.Data;

namespace CRM.UI.Converters
{
    public class PercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;

            if (value is double d)
                return $"{d}%";
            if (value is decimal m)
                return $"{m}%";
            if (value is int i)
                return $"{i}%";

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = value?.ToString()?.Replace("%", "").Trim();

            if (double.TryParse(str, NumberStyles.Any, culture, out double result))
                return result;

            return 0d;
        }
    }
}
