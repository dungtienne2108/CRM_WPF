using CommunityToolkit.Mvvm.ComponentModel;
using CRM.Application.Dtos.Employee;
using CRM.UI.ViewModels.Base;

namespace CRM.UI.ViewModels.Admin.EmployeeManagement
{
    public partial class EmployeeItemViewModel : ViewModelBase
    {
        [ObservableProperty]
        private bool _isSelected;
        [ObservableProperty]
        private EmployeeDto _employeeDto;

        public EmployeeItemViewModel(EmployeeDto employeeDto, int index)
        {
            EmployeeDto = employeeDto;
            Index = index;
        }

        public int Index { get; }

        public int Id => EmployeeDto.Id;
        public string Code => EmployeeDto.Code;
        public string Name => EmployeeDto.Name;
        public string Email => EmployeeDto.Email;
        public string PhoneNumber => EmployeeDto.PhoneNumber;
        public int GenderId => EmployeeDto.GenderId;
        public string GenderName => EmployeeDto.GenderName;
        public string? Address => EmployeeDto.Address;
        public int EmployeeTypeId => EmployeeDto.EmployeeTypeId;
        public string EmployeeTypeName => EmployeeDto.EmployeeTypeName;
    }
}
