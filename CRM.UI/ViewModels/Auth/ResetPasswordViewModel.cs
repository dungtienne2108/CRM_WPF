using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRM.Application.Interfaces.Auth;
using CRM.UI.ViewModels.Base;
using CRM.UI.Views.Auth;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
using System.Windows;

namespace CRM.UI.ViewModels.Auth
{
    public partial class ResetPasswordViewModel : ViewModelBase
    {
        private readonly string _email;
        private readonly IAuthenticationService _authenticationService;
        private readonly IServiceProvider _serviceProvider;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Vui lòng nhập mã xác thực")]
        private string _otp = string.Empty;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
        private string _newPassword = string.Empty;

        public ResetPasswordViewModel(string email, IAuthenticationService authenticationService, IServiceProvider serviceProvider)
        {
            _email = email;
            _authenticationService = authenticationService;
            _serviceProvider = serviceProvider;
        }

        [RelayCommand]
        private async Task ResetPasswordAsync()
        {
            if (string.IsNullOrEmpty(Otp))
            {
                SetError("Vui lòng nhập mã xác thực");
                return;
            }

            if (string.IsNullOrEmpty(NewPassword))
            {
                SetError("Vui lòng nhập mật khẩu mới");
                return;
            }

            if (!ForgotPasswordViewModel.ValidateOtp(_email, Otp))
            {
                SetError("OTP không hợp lệ hoặc đã hết hạn");
                return;
            }

            var result = await _authenticationService.ChangePasswordAsync(_email, NewPassword);
            if (result.IsSuccess)
            {
                ForgotPasswordViewModel.ClearOtp(_email);
                ClearAllErrors();

                MessageBox.Show("Đặt lại mật khẩu thành công. Vui lòng đăng nhập lại.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                var loginWindow = _serviceProvider.GetRequiredService<LoginWindow>();
                System.Windows.Application.Current.MainWindow = loginWindow;
                loginWindow.Show();

                System.Windows.Application.Current.Windows
                    .OfType<Window>()
                    .FirstOrDefault(x => x.DataContext == this)?
                    .Close();
            }
            else
            {
                MessageBox.Show($"Đặt lại mật khẩu thất bại: {result.Error.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);

                var forgotPasswordWindow = _serviceProvider.GetService<ForgotPasswordWindow>();
                System.Windows.Application.Current.MainWindow = forgotPasswordWindow;
                forgotPasswordWindow?.Show();

                System.Windows.Application.Current.Windows
                    .OfType<Window>()
                    .FirstOrDefault(x => x.DataContext == this)?
                    .Close();
            }

        }
    }
}
