using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRM.Application.Dtos.Auth;
using CRM.Application.Interfaces;
using CRM.Application.Interfaces.Auth;
using CRM.UI.Helpers;
using CRM.UI.Services.Navigation;
using CRM.UI.ViewModels.Base;
using CRM.UI.Views.Auth;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
using System.Windows;

namespace CRM.UI.ViewModels.Auth
{
    public partial class LoginViewModel : ViewModelBase
    {
        private readonly IAuthenticationService _authService;
        private readonly INavigationService _navigationService;
        private readonly Services.Dialog.IDialogService _dialogService;
        private readonly ISessionService _sessionService;
        private readonly IServiceProvider _serviceProvider;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Vui lòng nhập tài khoản")]
        [MaxLength(50, ErrorMessage = "Tài khoản không được vượt quá 50 ký tự")]
        [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ")]
        private string userName = string.Empty;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [MaxLength(100, ErrorMessage = "Mật khẩu không được vượt quá 100 ký tự")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        private string password = string.Empty;
        [ObservableProperty]
        private bool rememberMe;
        [ObservableProperty]
        private bool isPasswordVisible;
        [ObservableProperty]
        private bool isPasswordLoadedFromConfig = false;

        public LoginViewModel(
            IServiceProvider serviceProvider,
            IAuthenticationService authenticationService,
            ISessionService sessionService,
            INavigationService navigationService,
            Services.Dialog.IDialogService dialogService)
        {
            _authService = authenticationService;
            _navigationService = navigationService;
            _dialogService = dialogService;
            _sessionService = sessionService;
            _serviceProvider = serviceProvider;

            Title = "Login";

            IsLoading = false;

            var userConfig = ConfigManager.Load();
            if (userConfig.RememberMe)
            {
                UserName = userConfig.Username;
                Password = userConfig.Password;
                IsPasswordLoadedFromConfig = true;
                RememberMe = userConfig.RememberMe;
            }
        }

        [RelayCommand]
        private async Task LoginAsync()
        {
            ClearAllErrors();
            ValidateAllProperties();
            try
            {
                if (!ValidateInput())
                    return;

                IsLoading = true;
                ClearAllErrors();

                //string actualPassword = Password;
                //if (IsPasswordLoadedFromConfig)
                //{
                //    var userConfig = ConfigManager.Load();
                //    actualPassword = userConfig.Password;
                //}

                var loginRequest = new LoginRequestDto
                {
                    UserName = UserName,
                    Password = Password,
                    RememberMe = RememberMe
                };

                var result = await _authService.LoginAsync(loginRequest, "127.0.0.1", default);

                if (result.IsSuccess)
                {
                    _sessionService.SetCurrentAccount(result.Value.Account);
                    _sessionService.SetAccessToken(result.Value.AccessToken);
                    _sessionService.SetRefreshToken(result.Value.RefreshToken);

                    if (RememberMe)
                    {
                        var config = new UserConfig
                        {
                            Username = UserName,
                            Password = Password,
                            RememberMe = true
                        };
                        ConfigManager.Save(config);
                    }
                    else
                    {
                        IsPasswordLoadedFromConfig = false;
                        ConfigManager.Save(new UserConfig());
                    }

                    var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
                    System.Windows.Application.Current.MainWindow = mainWindow;
                    mainWindow.Show();

                    System.Windows.Application.Current.Windows
                        .OfType<Window>()
                        .FirstOrDefault(x => x.DataContext == this)?
                        .Close();
                }
                else
                {
                    SetError(result.Error.Message);
                    await _dialogService.ShowErrorAsync(result.Error.Message, "Lỗi");
                }
            }
            catch (Exception ex)
            {
                SetError($"Lỗi xảy ra khi đăng nhập : {ex.Message}");
                await _dialogService.ShowErrorAsync(ex.Message, "Lỗi");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void TogglePasswordVisibility()
        {
            IsPasswordVisible = !IsPasswordVisible;
        }

        [RelayCommand]
        private void ForgotPassword()
        {
            var forgotPasswordWindow = _serviceProvider.GetRequiredService<ForgotPasswordWindow>();
            System.Windows.Application.Current.MainWindow = forgotPasswordWindow;
            forgotPasswordWindow.Show();
            System.Windows.Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(x => x.DataContext == this)?
                .Close();
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrEmpty(UserName))
            {
                SetError("Vui lòng nhập tài khoản");
                MessageBox.Show("Vui lòng nhập tài khoản", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (string.IsNullOrEmpty(Password))
            {
                SetError("Vui lòng nhập mật khẩu");
                MessageBox.Show("Vui lòng nhập mật khẩu", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }
    }
}
