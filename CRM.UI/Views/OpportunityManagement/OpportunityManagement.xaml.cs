using CRM.UI.ViewModels.OpportunityManagement;
using System.Windows;
using System.Windows.Controls;

namespace CRM.UI.Views.OpportunityManagement
{
    /// <summary>
    /// Interaction logic for OpportunityManagement.xaml
    /// </summary>
    public partial class OpportunityManagement : UserControl
    {
        private readonly OpportunityManagementViewModel _viewModel;

        public OpportunityManagement(OpportunityManagementViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;

            this.Unloaded += Cleanup;
            this.Loaded += UserControl_Loaded;
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.InitializeAsync();
        }

        private void PageNumberTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
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

        private void Cleanup(object sender, RoutedEventArgs e)
        {
            this.Loaded -= UserControl_Loaded;
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.ContextMenu != null)
            {
                button.ContextMenu.PlacementTarget = button;
                button.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                button.ContextMenu.IsOpen = true;
            }
        }
    }
}

