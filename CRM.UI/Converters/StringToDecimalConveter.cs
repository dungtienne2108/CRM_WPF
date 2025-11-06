using System.Globalization;
using System.Windows.Data;

namespace CRM.UI.Converters
{
    public class StringToDecimalConveter : IValueConverter
    {
        //public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        //{
        //    return value?.ToString() ?? string.Empty;
        //}

        //public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        //{
        //    if (decimal.TryParse(value?.ToString(), out var result))
        //        return result;
        //    return 0; // hoặc Binding.DoNothing để tránh exception
        //}

        private static readonly CultureInfo VietNamCulture = new CultureInfo("vi-VN");

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }

            if (decimal.TryParse(value.ToString(), out decimal amount))
            {
                // Format dạng "230.000.000"
                return string.Format(VietNamCulture, "{0:N0}", amount);
            }

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return 0m;
            }

            // Loại bỏ các ký tự phân tách để parse lại
            string strValue = value.ToString()
                .Replace(".", "")
                .Replace(",", "")
                .Trim();

            if (decimal.TryParse(strValue, NumberStyles.Number, VietNamCulture, out decimal result))
            {
                return result;
            }

            return 0m;
        }
    }
}
