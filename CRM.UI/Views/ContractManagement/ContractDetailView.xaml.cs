using CRM.UI.ViewModels.ContractManagement;
using System.Windows;
using System.Windows.Controls;

namespace CRM.UI.Views.ContractManagement
{
    /// <summary>
    /// Interaction logic for ContractDetailView.xaml
    /// </summary>
    public partial class ContractDetailView : Window
    {
        private readonly ContractDetailViewModel _viewModel;

        public ContractDetailView(ContractDetailViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }

        private void AddImageButton_Click(object sender, RoutedEventArgs e)
        {
            // Open file dialog to select images
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Multiselect = true,
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filename in openFileDialog.FileNames)
                {
                    var documentImage = new DocumentImage
                    {
                        ImagePath = filename,
                        FileName = System.IO.Path.GetFileName(filename)
                    };
                }
            }
        }

        private void DeleteImageButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var documentImage = button?.DataContext as DocumentImage;
            if (documentImage != null)
            {
                // Remove the image from the collection
            }
        }

        private void ViewImageButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var documentImage = button?.DataContext as DocumentImage;
            if (documentImage != null)
            {

            }
        }
    }

    public class DocumentImage
    {
        public required string ImagePath { get; set; }
        public required string FileName { get; set; }
    }
}
