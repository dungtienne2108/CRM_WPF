using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;

namespace CRM.Infrastructure.Services
{
    public static class ExcelHelper
    {
        /// <summary>
        /// Xuất list ra file Excel với định dạng đẹp
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu của object trong list</typeparam>
        /// <param name="data">Danh sách dữ liệu cần xuất</param>
        /// <param name="sheetName">Tên sheet (mặc định: "Data")</param>
        /// <param name="title">Tiêu đề của bảng (tùy chọn)</param>
        /// <returns>Byte array của file Excel</returns>
        public static byte[] ExportToExcel<T>(
            List<T> data,
            string sheetName = "Data",
            string? title = null) where T : class
        {
            ExcelPackage.License.SetNonCommercialPersonal("FLC_CRM");

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add(sheetName);

                // Lấy các properties của object
                var properties = typeof(T).GetProperties()
                    .Where(p => p.CanRead && p.GetCustomAttribute<DisplayNameAttribute>() != null)
                    .ToList();

                int currentRow = 1;

                // Thêm tiêu đề nếu có
                if (!string.IsNullOrEmpty(title))
                {
                    worksheet.Cells[currentRow, 1, currentRow, properties.Count].Merge = true;
                    worksheet.Cells[currentRow, 1].Value = title;
                    worksheet.Cells[currentRow, 1].Style.Font.Size = 16;
                    worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
                    worksheet.Cells[currentRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[currentRow, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[currentRow, 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(68, 114, 196));
                    worksheet.Cells[currentRow, 1].Style.Font.Color.SetColor(Color.White);
                    worksheet.Row(currentRow).Height = 30;
                    currentRow += 2; // Thêm khoảng trống
                }

                // Thêm header
                int col = 1;
                foreach (var prop in properties)
                {
                    // Sử dụng DisplayName attribute nếu có, không thì dùng tên property
                    var displayName = prop.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? prop.Name;
                    worksheet.Cells[currentRow, col].Value = displayName;

                    // Style cho header
                    worksheet.Cells[currentRow, col].Style.Font.Bold = true;
                    worksheet.Cells[currentRow, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[currentRow, col].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(91, 155, 213));
                    worksheet.Cells[currentRow, col].Style.Font.Color.SetColor(Color.White);
                    worksheet.Cells[currentRow, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[currentRow, col].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[currentRow, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                    col++;
                }

                worksheet.Row(currentRow).Height = 25;
                currentRow++;

                // Thêm dữ liệu
                int startDataRow = currentRow;
                foreach (var item in data)
                {
                    col = 1;
                    foreach (var prop in properties)
                    {
                        var value = prop.GetValue(item);
                        worksheet.Cells[currentRow, col].Value = value;

                        // Format theo kiểu dữ liệu
                        if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?))
                        {
                            worksheet.Cells[currentRow, col].Style.Numberformat.Format = "dd/MM/yyyy";
                        }
                        else if (prop.PropertyType == typeof(decimal) || prop.PropertyType == typeof(decimal?) ||
                                 prop.PropertyType == typeof(double) || prop.PropertyType == typeof(double?))
                        {
                            worksheet.Cells[currentRow, col].Style.Numberformat.Format = "#,##0.00";
                        }
                        else if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(long))
                        {
                            worksheet.Cells[currentRow, col].Style.Numberformat.Format = "#,##0";
                        }

                        // Style cho data cells
                        worksheet.Cells[currentRow, col].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.LightGray);
                        worksheet.Cells[currentRow, col].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        // Màu xen kẽ cho các dòng
                        if (currentRow % 2 == 0)
                        {
                            worksheet.Cells[currentRow, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[currentRow, col].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(242, 242, 242));
                        }

                        col++;
                    }
                    currentRow++;
                }

                // Auto-fit columns
                for (int i = 1; i <= properties.Count; i++)
                {
                    worksheet.Column(i).AutoFit();
                    // Đảm bảo chiều rộng tối thiểu
                    if (worksheet.Column(i).Width < 15)
                    {
                        worksheet.Column(i).Width = 15;
                    }
                }

                // Thêm filter cho header
                if (data.Count > 0)
                {
                    var headerRow = title != null ? 3 : 1;
                    worksheet.Cells[headerRow, 1, currentRow - 1, properties.Count].AutoFilter = true;
                }

                // Đóng băng header
                worksheet.View.FreezePanes(title != null ? 4 : 2, 1);

                return package.GetAsByteArray();
            }
        }

        /// <summary>
        /// Xuất list ra file Excel và lưu vào đường dẫn
        /// </summary>
        public static void ExportToExcelFile<T>(
            List<T> data,
            string filePath,
            string sheetName = "Data",
            string? title = null) where T : class
        {
            var excelData = ExportToExcel(data, sheetName, title);
            File.WriteAllBytes(filePath, excelData);
        }

        /// <summary>
        /// Xuất nhiều sheets trong một file Excel
        /// </summary>
        public static byte[] ExportMultipleSheetsToExcel<T>(
            Dictionary<string, List<T>> dataSheets,
            string? mainTitle = null) where T : class
        {
            ExcelPackage.License.SetNonCommercialPersonal("FLC_CRM");

            using (var package = new ExcelPackage())
            {
                foreach (var sheet in dataSheets)
                {
                    var worksheet = package.Workbook.Worksheets.Add(sheet.Key);
                    FillWorksheet(worksheet, sheet.Value, sheet.Key, mainTitle);
                }

                return package.GetAsByteArray();
            }
        }

        private static void FillWorksheet<T>(
            ExcelWorksheet worksheet,
            List<T> data,
            string sheetName,
            string? title = null) where T : class
        {
            var properties = typeof(T).GetProperties()
                .Where(p => p.CanRead)
                .ToList();

            int currentRow = 1;

            if (!string.IsNullOrEmpty(title))
            {
                worksheet.Cells[currentRow, 1, currentRow, properties.Count].Merge = true;
                worksheet.Cells[currentRow, 1].Value = title;
                worksheet.Cells[currentRow, 1].Style.Font.Size = 16;
                worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
                worksheet.Cells[currentRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[currentRow, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[currentRow, 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(68, 114, 196));
                worksheet.Cells[currentRow, 1].Style.Font.Color.SetColor(Color.White);
                worksheet.Row(currentRow).Height = 30;
                currentRow += 2;
            }

            // Header
            int col = 1;
            foreach (var prop in properties)
            {
                var displayName = prop.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? prop.Name;
                worksheet.Cells[currentRow, col].Value = displayName;
                worksheet.Cells[currentRow, col].Style.Font.Bold = true;
                worksheet.Cells[currentRow, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[currentRow, col].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(91, 155, 213));
                worksheet.Cells[currentRow, col].Style.Font.Color.SetColor(Color.White);
                worksheet.Cells[currentRow, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[currentRow, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                col++;
            }

            worksheet.Row(currentRow).Height = 25;
            currentRow++;

            // Data
            foreach (var item in data)
            {
                col = 1;
                foreach (var prop in properties)
                {
                    var value = prop.GetValue(item);
                    worksheet.Cells[currentRow, col].Value = value;

                    if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?))
                    {
                        worksheet.Cells[currentRow, col].Style.Numberformat.Format = "dd/MM/yyyy";
                    }
                    else if (prop.PropertyType == typeof(decimal) || prop.PropertyType == typeof(decimal?) ||
                             prop.PropertyType == typeof(double) || prop.PropertyType == typeof(double?))
                    {
                        worksheet.Cells[currentRow, col].Style.Numberformat.Format = "#,##0.00";
                    }

                    worksheet.Cells[currentRow, col].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.LightGray);

                    if (currentRow % 2 == 0)
                    {
                        worksheet.Cells[currentRow, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[currentRow, col].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(242, 242, 242));
                    }

                    col++;
                }
                currentRow++;
            }

            // Auto-fit
            for (int i = 1; i <= properties.Count; i++)
            {
                worksheet.Column(i).AutoFit();
                if (worksheet.Column(i).Width < 15)
                {
                    worksheet.Column(i).Width = 15;
                }
            }

            if (data.Count > 0)
            {
                var headerRow = title != null ? 3 : 1;
                worksheet.Cells[headerRow, 1, currentRow - 1, properties.Count].AutoFilter = true;
            }

            worksheet.View.FreezePanes(title != null ? 4 : 2, 1);
        }
    }
}
