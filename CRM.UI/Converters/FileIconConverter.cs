using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace CRM.UI.Converters
{
    public class FileIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string fileName)
            {
                var ext = Path.GetExtension(fileName).ToLowerInvariant();

                return ext switch
                {
                    ".pdf" => "FilePdfBox",
                    ".doc" or ".docx" => "FileWord",
                    ".xls" or ".xlsx" => "FileExcel",
                    ".png" or ".jpg" or ".jpeg" or ".bmp" => "FileImage",
                    ".txt" => "FileDocument",
                    _ => "File"
                };
            }

            return "File";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
