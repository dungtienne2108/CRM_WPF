using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRM.Application.Dtos.Employee;
using CRM.Application.Interfaces.Employee;
using CRM.UI.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows;

namespace CRM.UI.ViewModels.Admin.EmployeeManagement
{
    public partial class EmployeeDetailViewModel : ViewModelBase
    {
        private readonly IEmployeeService _employeeService;

        [ObservableProperty]
        private EmployeeItemViewModel _employee;

        [ObservableProperty]
        private int _employeeId;
        [ObservableProperty]
        private string _employeeName;
        [ObservableProperty]
        private string _employeeIdentityCard;
        [ObservableProperty]
        private string? _employeeDescription;
        [ObservableProperty]
        private string _employeeCode;
        [ObservableProperty]
        private string _employeeLevel;
        [ObservableProperty]
        private int _employeeLevelId;
        [ObservableProperty]
        private string _employeeEmail;
        [ObservableProperty]
        private string _employeePhone;
        [ObservableProperty]
        private string _gender;
        [ObservableProperty]
        private int _genderId;
        [ObservableProperty]
        private string _employeeAddress;
        [ObservableProperty]
        private DateTime? _employeeBirthday;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsDeleteVisible))]
        private bool _isEditMode = false;

        public bool IsDeleteVisible => !IsEditMode;

        [ObservableProperty]
        private ObservableCollection<GenderOption> _genderOptions = new();
        [ObservableProperty]
        private ObservableCollection<EmployeeLevelOption> _employeeLevelOptions = new();

        public EmployeeDetailViewModel(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        #region Public method
        public async Task LoadDataAsync(EmployeeItemViewModel employee)
        {
            if (employee == null)
            {
                MessageBox.Show("Nhân viên không tồn tại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Employee = employee;
            EmployeeId = employee.Id;
            await GetEmployeeAsync(EmployeeId);
            await GetGendersAsync();
            await GetEmployeeLevelsAsync();
        }
        #endregion

        #region Commands
        [RelayCommand]
        private void ToggleEditMode()
        {
            IsEditMode = !IsEditMode;
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            ClearAllErrors();
            ValidateAllProperties();

            if (HasErrors)
            {
                MessageBox.Show("Vui lòng sửa các lỗi trước khi lưu.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            await UpdateEmployeeAsync();
        }

        [RelayCommand]
        private async Task DeleteAsync()
        {
            var result = MessageBox.Show(
                        "Bạn có chắc chắn muốn xóa nhân viên này không?",
                        "Xác nhận xóa",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                await DeleteEmployeeAsync(EmployeeId);
            }
        }
        #endregion

        #region Private method
        private async Task UpdateEmployeeAsync()
        {
            try
            {
                var updateRequest = new UpdateEmployeeRequest
                {
                    Id = EmployeeId,
                    Name = EmployeeName,
                    IdentityCard = EmployeeIdentityCard,
                    Email = EmployeeEmail,
                    Phone = EmployeePhone,
                    GenderId = GenderId,
                    LevelId = EmployeeLevelId,
                    Address = EmployeeAddress,
                    Description = EmployeeDescription,
                    DateOfBirth = EmployeeBirthday.HasValue ? EmployeeBirthday.Value : DateTime.MinValue
                };
                var updateResult = await _employeeService.UpdateEmployeeAsync(updateRequest);
                if (updateResult.IsSuccess)
                {
                    MessageBox.Show("Cập nhật thông tin nhân viên thành công.");
                    IsEditMode = false;
                    await GetEmployeeAsync(EmployeeId);
                }
                else
                {
                    MessageBox.Show("Lỗi cập nhật thông tin nhân viên: " + updateResult.Error.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật thông tin nhân viên: " + ex.Message);
            }
        }

        private async Task DeleteEmployeeAsync(int employeeId)
        {
            try
            {
                if (employeeId <= 0)
                {
                    MessageBox.Show("Không tìm thấy nhân viên", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var res = await _employeeService.DeleteEmployeeAsync(employeeId);
                if (res.IsSuccess)
                {
                    MessageBox.Show("Xóa thành công nhân viên", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                    System.Windows.Application.Current.Windows
                        .OfType<Window>()
                        .SingleOrDefault(w => w.DataContext == this)?
                        .Close();
                }
                else
                {
                    MessageBox.Show("Lỗi khi xóa nhân viên", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            catch { }
        }

        private async Task GetEmployeeAsync(int employeeId)
        {
            try
            {
                var employeeResult = await _employeeService.GetEmployeeByIdAsync(employeeId);
                if (employeeResult.IsSuccess)
                {
                    var employee = employeeResult.Value;
                    EmployeeId = employee.Id;
                    EmployeeName = employee.Name;
                    EmployeeIdentityCard = employee.IdentityCard;
                    EmployeeCode = employee.Code;
                    EmployeeEmail = employee.Email;
                    EmployeePhone = employee.PhoneNumber;
                    GenderId = employee.GenderId;
                    EmployeeLevelId = employee.EmployeeTypeId;
                    EmployeeAddress = employee.Address;
                    EmployeeDescription = employee.Description;
                    EmployeeBirthday = employee.Birthday;
                    EmployeeLevel = employee.EmployeeTypeName;
                    Gender = employee.GenderName;
                }
                else
                {
                    MessageBox.Show("Lỗi tải thông tin nhân viên: " + employeeResult.Error.Message);
                }
            }
            catch (Exception)
            {

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
    }
}
