using CRM.Application.Dtos.Lead;
using CRM.UI.ViewModels.LeadManagement;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CRM.UI.Views.LeadManagement
{
    /// <summary>
    /// Interaction logic for LeadManagementView.xaml
    /// </summary>
    public partial class LeadManagementView : UserControl
    {
        private readonly LeadManagementViewModel _viewModel;
        private readonly IServiceProvider _serviceProvider;

        public LeadManagementView(
            LeadManagementViewModel viewModel,
            IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;

            this.Unloaded += Cleanup;
            Loaded += UserControl_Loaded;

            SubscribeToViewModelEvents();
            _serviceProvider = serviceProvider;
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.InitializeAsync();
        }

        private void SubscribeToViewModelEvents()
        {
            _viewModel.StatusChanged += OnStatusChanged;
            _viewModel.LeadsRemoved += OnLeadsRemoved;
        }

        private void OnStatusChanged(LeadDto leadDto, int newStatus)
        {

            MessageBox.Show($"Đã cập nhật trạng thái của {leadDto.Name} thành '{newStatus}'",
                          "Cập nhật thành công", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OnLeadsRemoved(List<LeadDto> removedLeads)
        {
            MessageBox.Show($"Đã xóa {removedLeads.Count} khách hàng tiềm năng",
                          "Xóa thành công", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #region UI Event Handlers

        private void ActionButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.ContextMenu != null)
            {
                button.ContextMenu.PlacementTarget = button;
                button.ContextMenu.IsOpen = true;
            }
        }

        private async void StatusMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem && menuItem.Tag is string statusId)
            {
                // Find the context menu and then the button
                var contextMenu = menuItem.Parent as ContextMenu;
                var button = contextMenu?.PlacementTarget as Button;
                var leadItem = button?.DataContext as LeadItemViewModel;

                if (leadItem != null)
                {
                    // Execute command through ViewModel
                    if (_viewModel.ChangeStatusCommand.CanExecute(null))
                    {
                        int newStatusId = int.TryParse(statusId, out int id) ? id : 1;
                        await _viewModel.ChangeStatusCommand.ExecuteAsync(new object[] { leadItem, newStatusId });
                    }
                }
            }
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

        private void PageNumberTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back || e.Key == Key.Delete)
            {
                var tb = (TextBox)sender;
                if (tb.Text.Length <= 1)
                {
                    e.Handled = true;
                }
            }
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGridRow row && row.Item is LeadItemViewModel item)
            {
                //var detailWindow = new LeadDetailView();
                //detailWindow.ShowDialog();
            }
        }



        #endregion

        #region Pulic

        //public void LoadLeads(System.Collections.Generic.IEnumerable<LeadDto> leads)
        //{
        //    _viewModel.LoadLeads(leads);
        //}

        //public void AddLead(LeadDto lead)
        //{
        //    _viewModel.AddLead(lead);
        //}

        //public void RemoveSelectedLeads()
        //{
        //    _viewModel.RemoveSelectedLeads();
        //}

        public System.Collections.Generic.List<LeadDto> GetSelectedLeads()
        {
            return _viewModel.GetSelectedLeads();
        }

        public System.Collections.Generic.List<LeadDto> GetAllLeads()
        {
            return _viewModel.GetAllLeads();
        }

        //public void RefreshData()
        //{
        //    _viewModel.RefreshData();
        //}

        #endregion

        #region Cleanup

        public void Cleanup(object sender, RoutedEventArgs e)
        {
            _viewModel.StatusChanged -= OnStatusChanged;
            _viewModel.LeadsRemoved -= OnLeadsRemoved;
        }

        #endregion
    }
}
