using System.Globalization;
using System.Windows.Data;

namespace CRM.UI.Converters
{
    public class CurrencyFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal || value is double || value is int)
            {
                return string.Format(new CultureInfo("vi-VN"), "{0:N0}", value);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str && decimal.TryParse(str, NumberStyles.Any, new CultureInfo("vi-VN"), out decimal result))
            {
                return result;
            }
            return 0;
        }
    }
}
