using CRM.UI.Helpers;
using System.Windows;
using System.Windows.Controls;

namespace CRM.UI.Views.Dashboard
{
    /// <summary>
    /// Interaction logic for DashboardView.xaml
    /// </summary>
    public partial class DashboardView : UserControl
    {
        private bool _isInitialized = false;

        public DashboardView()
        {
            InitializeComponent();
            Loaded += LoadAsync;
            Unloaded += UserControl_Unloaded;
        }

        private async void LoadAsync(object sender, RoutedEventArgs e)
        {
            // Bỏ điều kiện static, chỉ kiểm tra instance hiện tại
            if (!_isInitialized)
            {
                try
                {
                    var env = await WebViewEnvironmentHelper.GetEnvironmentAsync();
                    await PowerBiWebView.EnsureCoreWebView2Async(env);
                    string htmlPath = System.IO.Path.Combine(
                        System.AppDomain.CurrentDomain.BaseDirectory,
                        "Resources",
                        "report.html");
                    PowerBiWebView.Source = new System.Uri(htmlPath, System.UriKind.Absolute);
                    _isInitialized = true;
                }
                catch (Exception ex)
                {
                    // Log lỗi nếu cần
                    System.Diagnostics.Debug.WriteLine($"WebView2 initialization error: {ex.Message}");
                }
            }
            else
            {
                // Nếu đã khởi tạo rồi, refresh lại nội dung
                if (PowerBiWebView.CoreWebView2 != null)
                {
                    PowerBiWebView.Reload();
                }
                else
                {
                    // Nếu CoreWebView2 bị null, khôi phục lại
                    var env = await WebViewEnvironmentHelper.GetEnvironmentAsync();
                    await PowerBiWebView.EnsureCoreWebView2Async(env);
                    string htmlPath = System.IO.Path.Combine(
                        System.AppDomain.CurrentDomain.BaseDirectory,
                        "Resources",
                        "report.html");
                    PowerBiWebView.Source = new System.Uri(htmlPath, System.UriKind.Absolute);
                }
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            // Không cần handle event, để WebView tự quản lý lifecycle
            // e.Handled = true; // Bỏ dòng này
        }
    }
}
