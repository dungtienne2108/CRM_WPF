using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRM.Application.Dtos.User;
using CRM.Application.Interfaces;
using CRM.UI.ViewModels.Base;
using CRM.UI.Views.Dashboard;
using Microsoft.Extensions.DependencyInjection;

namespace CRM.UI.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly ISessionService _sessionService;
        private readonly IServiceProvider _serviceProvider;
        private object _currentView;

        [ObservableProperty]
        private AccountDto? _currentAccount;
        [ObservableProperty]
        private bool _isAdmin;

        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged(nameof(CurrentView));
            }
        }

        public MainWindowViewModel(ISessionService sessionService, IServiceProvider serviceProvider)
        {
            CurrentView = new DashboardView();
            _sessionService = sessionService;

            _ = LoadUserAsync();
            _serviceProvider = serviceProvider;
        }

        public async Task LoadUserAsync()
        {
            try
            {
                IsBusy = true;
                CurrentAccount = _sessionService.CurrentAccount;
                IsAdmin = CurrentAccount.AccountTypeId == 1;
                ClearAllErrors();
            }
            catch (Exception ex)
            {
                SetError($"Lỗi khi lấy account hiện tại : {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void UserInfo()
        {
            var userInfoView = _serviceProvider.GetRequiredService<UserInfoView>();
            userInfoView.ShowDialog();
        }
    }
}
