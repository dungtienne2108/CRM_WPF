using System.Windows;
using System.Windows.Controls;

namespace CRM.UI.Views.Dashboard
{
    /// <summary>
    /// Interaction logic for DashboardView.xaml
    /// </summary>
    public partial class DashboardView : UserControl
    {
        public DashboardView()
        {
            InitializeComponent();

            Loaded += LoadAsync;
        }

        private async void LoadAsync(object sender, RoutedEventArgs e)
        {
            await PowerBiWebView.EnsureCoreWebView2Async(null);

            string htmlPath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Resources", "report.html");
            PowerBiWebView.Source = new Uri(htmlPath, UriKind.Absolute);
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            var powerBiWindow = new PowerBiEmbedded();
            powerBiWindow.Show();
        }
    }
}
