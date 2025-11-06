using CRM.UI.ViewModels.Dashboard;
using System.Windows;

namespace CRM.UI.Views.Dashboard
{
    /// <summary>
    /// Interaction logic for UserInfoView.xaml
    /// </summary>
    public partial class UserInfoView : Window
    {
        private readonly UserInfoViewModel _viewModel;

        public UserInfoView(UserInfoViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }
    }
}
