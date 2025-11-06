using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRM.Application.Interfaces.Email;
using CRM.UI.ViewModels.Base;
using CRM.UI.Views.Auth;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using IDialogService = CRM.UI.Services.Dialog.IDialogService;

namespace CRM.UI.ViewModels.Auth
{
    public partial class ForgotPasswordViewModel : ViewModelBase
    {
        private readonly IEmailService _emailService;
        private readonly IDialogService _dialogService;
        private readonly IServiceProvider _serviceProvider;
        private static Dictionary<string, string> _resetPasswordOtp = new();

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        private string _email = string.Empty;


        public ForgotPasswordViewModel(IEmailService emailService, IDialogService dialogService, IServiceProvider serviceProvider)
        {
            _emailService = emailService;
            _dialogService = dialogService;
            _serviceProvider = serviceProvider;
        }

        [RelayCommand]
        private async Task SendResetPasswordEmailAsync()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                SetError("Vui lòng nhập email.");
                return;
            }

            var otp = Random.Shared.Next(100000, 999999).ToString();
            _resetPasswordOtp[Email] = otp;
            var body = $@"
                <h2>Đặt lại mật khẩu</h2>
                <p>Mã xác thực để đặt lại mật khẩu của bạn là:</p>
                <h3>{otp}</h3>
            ";

            try
            {
                await _emailService.SendEmailAsync(Email, "Yêu cầu đặt lại mật khẩu", body);
                ClearAllErrors();

                await _dialogService.ShowConfirmationAsync("Vui lòng kiểm tra email để lấy mã xác thực.", "Gửi email thành công");

                var vmFactory = _serviceProvider.GetRequiredService<Func<string, ResetPasswordViewModel>>();
                var window = new ResetPasswordWindow(vmFactory(Email));
                window.Show();

                System.Windows.Application.Current.Windows
                        .OfType<Window>()
                        .FirstOrDefault(x => x.DataContext == this)?
                        .Close();
            }
            catch (Exception ex)
            {
                SetError($"Lỗi xảy ra khi gửi email: {ex.Message}");
                await _dialogService.ShowErrorAsync(ex.Message, "Lỗi");
            }
        }

        public static bool ValidateOtp(string email, string otp)
        {
            return _resetPasswordOtp.ContainsKey(email) && _resetPasswordOtp[email] == otp;
        }

        public static void ClearOtp(string email)
        {
            if (_resetPasswordOtp.ContainsKey(email))
            {
                _resetPasswordOtp.Remove(email);
            }
        }
    }
}
