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
        private static bool _isInitialized = false;

        public DashboardView()
        {
            InitializeComponent();

            Loaded += LoadAsync;
        }

        private async void LoadAsync(object sender, RoutedEventArgs e)
        {
            if (!_isInitialized)
            {
                var env = await WebViewEnvironmentHelper.GetEnvironmentAsync();
                await PowerBiWebView.EnsureCoreWebView2Async(env);

                string htmlPath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Resources", "report.html");
                PowerBiWebView.Source = new System.Uri(htmlPath, System.UriKind.Absolute);

                _isInitialized = true;
            }
            else
            {
                // Nếu đã khởi tạo rồi, chỉ cần WebView2 tải lại
                if (PowerBiWebView.CoreWebView2 == null)
                {
                    var env = await WebViewEnvironmentHelper.GetEnvironmentAsync();
                    await PowerBiWebView.EnsureCoreWebView2Async(env);
                }
            }
        }
    }
}
