using System.IO;
using System.Windows;

namespace CRM.UI.Views.ContractManagement
{
    /// <summary>
    /// Interaction logic for DocumentPreviewWindow.xaml
    /// </summary>
    public partial class DocumentPreviewWindow : Window
    {
        private readonly string _filePath;
        private readonly string _contentType;

        public DocumentPreviewWindow(string filePath, string? contentType = null)
        {
            InitializeComponent();
            _filePath = filePath;
            _contentType = contentType;
            //Loaded += Window_Loaded;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(_filePath))
            {
                MessageBox.Show("Không tìm thấy file.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }

            var ext = Path.GetExtension(_filePath)?.ToLowerInvariant();

            if (IsImage(ext, _contentType))
            {
                PreviewImage.Visibility = Visibility.Visible;
                PreviewImage.Source = new System.Windows.Media.Imaging.BitmapImage(new System.Uri(_filePath));
            }
            else if (IsText(ext, _contentType))
            {
                PreviewText.Text = File.ReadAllText(_filePath);
                TextContainer.Visibility = Visibility.Visible;
            }
            else
            {
                await WebViewer.EnsureCoreWebView2Async();

                if (ext == ".pdf")
                {
                    WebViewer.Source = new Uri(_filePath);
                }
                else if (ext == ".docx" || ext == ".xlsx" || ext == ".pptx")
                {
                    var encoded = System.Web.HttpUtility.UrlEncode(_filePath);
                    WebViewer.Source = new Uri($"https://view.officeapps.live.com/op/view.aspx?src={encoded}");
                }
                else
                {
                    WebViewer.Source = new Uri(_filePath);
                }

                WebViewer.Visibility = Visibility.Visible;
            }
        }

        private bool IsImage(string ext, string contentType)
        {
            return contentType?.StartsWith("image") == true ||
        ext is ".jpg" or ".jpeg" or ".png" or ".gif" or ".bmp";
        }

        private bool IsText(string ext, string contentType)
        {
            return contentType?.StartsWith("text") == true ||
            ext is ".txt" or ".csv" or ".json" or ".xml" or ".log";
        }
    }
}
