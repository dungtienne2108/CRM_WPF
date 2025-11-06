using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRM.Application.Dtos.Employee;
using CRM.Application.Interfaces.Employee;
using CRM.UI.ViewModels.Base;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;

namespace CRM.UI.ViewModels.Admin.EmployeeManagement
{
    public partial class AddEmployeeViewModel : ViewModelBase
    {
        private readonly IEmployeeService _employeeService;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Tên nhân viên không được để trống")]
        [MinLength(3, ErrorMessage = "Tên nhân viên phải có ít nhất 3 ký tự")]
        private string _employeeName = string.Empty;
        [ObservableProperty]
        private int _employeeLevelId;
        [ObservableProperty]
        private int _genderId;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Số căn cước công dân không được để trống")]
        [MinLength(9, ErrorMessage = "Chứng minh nhân dân phải có ít nhất 9 ký tự")]
        private string _employeeIdentityCard = string.Empty;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        private string _employeeEmail = string.Empty;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        private string _employeePhone = string.Empty;
        [ObservableProperty]
        private string _employeeAddress = string.Empty;
        [ObservableProperty]
        private string? _employeeDescription = string.Empty;
        [ObservableProperty]
        private DateTime _employeeBirthday = DateTime.UtcNow;

        [ObservableProperty]
        private ObservableCollection<GenderOption> _genderOptions = new();
        [ObservableProperty]
        private ObservableCollection<EmployeeLevelOption> _employeeLevelOptions = new();

        public AddEmployeeViewModel(IEmployeeService employeeService)
        {
            _employeeService = employeeService;

            ErrorsChanged += (s, e) => OnPropertyChanged(nameof(CanSave));
        }

        #region Public Methods
        public async Task LoadDataAsync()
        {
            await GetGendersAsync();
            await GetEmployeeLevelsAsync();
        }
        #endregion

        #region Commands
        public bool CanSave => !HasErrors;

        [RelayCommand(CanExecute = nameof(CanSave))]
        private async Task SaveAsync()
        {
            ClearAllErrors();
            ValidateAllProperties();

            await CreateEmployeeAsync();
        }
        #endregion

        #region Private Methods
        private async Task CreateEmployeeAsync()
        {
            var createEmployeeRequest = new CreateEmployeeRequest
            {
                EmployeeName = EmployeeName,
                EmployeeLevelId = EmployeeLevelId,
                GenderId = GenderId,
                EmployeeIdentityCard = EmployeeIdentityCard,
                EmployeeEmail = EmployeeEmail,
                EmployeePhoneNumber = EmployeePhone,
                EmployeeAddress = EmployeeAddress,
                EmployeeBirthday = EmployeeBirthday,
                EmployeeDescription = EmployeeDescription
            };

            try
            {
                var result = await _employeeService.CreateEmployeeAsync(createEmployeeRequest);
                if (result.IsSuccess)
                {
                    MessageBox.Show("Tạo nhân viên thành công!");
                    // đóng
                    System.Windows.Application.Current.Windows
                        .OfType<Window>()
                        .SingleOrDefault(w => w.DataContext == this)?
                        .Close();
                }
                else
                {
                    MessageBox.Show("Tạo nhân viên thất bại: " + result.Error.Message);
                    System.Windows.Application.Current.Windows
                        .OfType<Window>()
                        .SingleOrDefault(w => w.DataContext == this)?
                        .Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tạo nhân viên: " + ex.Message);
            }
        }

        private async Task GetGendersAsync()
        {
            try
            {
                var gendersResult = await _employeeService.GetAllGendersAsync();

                if (gendersResult.IsSuccess)
                {
                    foreach (var item in gendersResult.Value)
                    {
                        GenderOptions.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải giới tính: " + ex.Message);
                // đóng
                System.Windows.Application.Current.Windows
                        .OfType<Window>()
                        .SingleOrDefault(w => w.DataContext == this)?
                        .Close();
            }
        }

        private async Task GetEmployeeLevelsAsync()
        {
            try
            {
                var levelsResult = await _employeeService.GetAllEmployeeLevelsAsync();
                if (levelsResult.IsSuccess)
                {
                    foreach (var item in levelsResult.Value)
                    {
                        EmployeeLevelOptions.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải cấp bậc nhân viên: " + ex.Message);
                // đóng
                System.Windows.Application.Current.Windows
                        .OfType<Window>()
                        .SingleOrDefault(w => w.DataContext == this)?
                        .Close();
            }
        }
        #endregion

        #region Property changed
        #endregion
    }
}
