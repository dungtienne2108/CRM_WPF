using System.Globalization;
using System.Windows.Data;
using Color = System.Windows.Media.Color;

namespace CRM.UI.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isCompleted)
            {
                return isCompleted ? Color.FromArgb(255, 33, 150, 243) : Color.FromArgb(255, 189, 189, 189);
            }
            return Color.FromArgb(255, 189, 189, 189);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StatusToIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                return status switch
                {
                    "Mở - Chưa liên lạc" => 0,
                    "Đang thực hiện - Đã liên lạc" => 1,
                    "Đóng - Chuyển đổi" => 2,
                    "Đóng - Không chuyển đổi" => 3,
                    _ => 0
                };
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
