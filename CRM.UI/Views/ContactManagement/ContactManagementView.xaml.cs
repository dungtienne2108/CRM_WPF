using CRM.UI.ViewModels.ContactManagement;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CRM.UI.Views.ContactManagement
{
    /// <summary>
    /// Interaction logic for ContactManagementView.xaml
    /// </summary>
    public partial class ContactManagementView : UserControl
    {
        private readonly ContactManagementViewModel _viewModel;

        public ContactManagementView(ContactManagementViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;

            Loaded += UserControl_Loaded;
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.InitializeAsync();
        }

        private void PageNumberTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (sender is TextBox textBox)
                {
                    var pageNumber = int.TryParse(textBox.Text, out int number) ? number : 1;
                    if (_viewModel.GoToPageCommand.CanExecute(pageNumber))
                    {
                        _viewModel.GoToPageCommand.Execute(pageNumber);
                        textBox.Text = _viewModel.CurrentPage.ToString();
                    }
                }
            }
        }
    }
}
