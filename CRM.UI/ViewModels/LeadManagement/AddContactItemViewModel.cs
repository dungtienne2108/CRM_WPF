using CommunityToolkit.Mvvm.ComponentModel;
using CRM.UI.ViewModels.Base;

namespace CRM.UI.ViewModels.LeadManagement
{
    public partial class AddContactItemViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _name = string.Empty;
        [ObservableProperty]
        private string _phone = string.Empty;
        [ObservableProperty]
        private string _email = string.Empty;
        [ObservableProperty]
        private string _address = string.Empty;
        [ObservableProperty]
        private int _salutationId;
        [ObservableProperty]
        private string _role = string.Empty;
        [ObservableProperty]
        private string _description = string.Empty;

        [ObservableProperty]
        private string? _validationMessage = string.Empty;

        public string? ValidateContactInput()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(Name))
            {
                errors.Add("Vui lòng nhập tên liên hệ.");
            }

            if (!string.IsNullOrWhiteSpace(Email))
            {
                var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                if (!System.Text.RegularExpressions.Regex.IsMatch(Email, emailPattern))
                {
                    errors.Add("Địa chỉ email không hợp lệ.");
                }
            }

            if (!string.IsNullOrWhiteSpace(Phone))
            {
                var phonePattern = @"^\+?[0-9\s\-()]{7,15}$";
                if (!System.Text.RegularExpressions.Regex.IsMatch(Phone, phonePattern))
                {
                    errors.Add("Số điện thoại không hợp lệ.");
                }
            }

            if (string.IsNullOrEmpty(Address))
            {
                errors.Add("Vui lòng nhập địa chỉ.");
            }

            if (SalutationId <= 0)
            {
                errors.Add("Vui lòng chọn xưng hô.");
            }

            if (string.IsNullOrEmpty(Role))
            {
                errors.Add("Vui lòng nhập chức vụ");
            }

            if (errors.Count == 0)
            {
                return null;
            }

            return string.Join("\n", errors);
        }
    }
}
