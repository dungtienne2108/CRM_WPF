using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRM.Application.Interfaces;
using CRM.UI.ViewModels.Base;
using CRM.UI.Views.Auth;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace CRM.UI.ViewModels.Dashboard
{
    public partial class UserInfoViewModel : ViewModelBase
    {
        private readonly ISessionService _sessionService;
        private readonly IServiceProvider _serviceProvider;

        [ObservableProperty]
        private string _name = string.Empty;
        [ObservableProperty]
        private string _birthDate = string.Empty;
        [ObservableProperty]
        private string _email = string.Empty;
        [ObservableProperty]
        private string _phoneNumber = string.Empty;

        public UserInfoViewModel(ISessionService sessionService, IServiceProvider serviceProvider)
        {
            _sessionService = sessionService;
            _serviceProvider = serviceProvider;
            LoadUserInfo();
        }

        private void LoadUserInfo()
        {
            try
            {
                IsBusy = true;
                var currentAccount = _sessionService.CurrentAccount;
                if (currentAccount != null)
                {
                    Name = currentAccount.EmployeeName ?? string.Empty;
                    BirthDate = currentAccount.EmployeeBirthDay?.ToString("dd/MM/yyyy") ?? string.Empty;
                    Email = currentAccount.EmployeeEmail ?? string.Empty;
                    PhoneNumber = currentAccount.EmployeePhone ?? string.Empty;
                }
                ClearAllErrors();
            }
            catch (Exception ex)
            {
                SetError($"Lỗi khi lấy thông tin người dùng: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void Logout()
        {
            try
            {
                IsBusy = true;
                _sessionService.SetCurrentAccount(null);
                _sessionService.SetAccessToken(string.Empty);
                _sessionService.SetRefreshToken(string.Empty);

                ClearAllErrors();

                // hỏi trước khi đăng xuất
                var result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận đăng xuất", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes)
                {

                    return;
                }

                var loginWindow = _serviceProvider.GetRequiredService<LoginWindow>();
                loginWindow.Show();

                System.Windows.Application.Current.Windows
                        .OfType<Window>()
                        .FirstOrDefault(x => x.DataContext == this)?
                        .Close();

                App.Current.MainWindow.Close();
            }
            catch (Exception ex)
            {
                SetError($"Lỗi khi đăng xuất: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
