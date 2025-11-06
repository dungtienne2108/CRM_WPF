using System.Globalization;
using System.Windows.Data;

namespace CRM.UI.Converters
{
    public class StringToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;

            int intValue = (int)value;
            return intValue == 0 ? string.Empty : intValue.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace(value?.ToString()))
                return 0; // Hoặc return null nếu property cho phép nullable

            if (int.TryParse(value.ToString(), out int result))
                return result;

            return 0; // Trả về 0 thay vì throw exception
        }
    }

}
