using System.Windows;
using System.Windows.Media.Imaging;

namespace CRM.UI.Views.ContractManagement
{
    /// <summary>
    /// Interaction logic for ImagePreviewWindow.xaml
    /// </summary>
    public partial class ImagePreviewWindow : Window
    {
        public ImagePreviewWindow(string imagePath)
        {
            InitializeComponent();

            this.WindowState = WindowState.Maximized;

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(imagePath);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            bitmap.Freeze();

            PreviewImage.Source = bitmap;
        }
    }
}
