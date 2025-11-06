using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRM.Application.Dtos.Lead;
using CRM.Application.Interfaces.Leads;
using CRM.UI.ViewModels.Base;
using CRM.UI.Views.LeadManagement;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Windows;

namespace CRM.UI.ViewModels.LeadManagement
{
    public partial class LeadItemViewModel : ViewModelBase
    {
        private readonly ILeadService _leadService;
        private readonly IServiceProvider _serviceProvider;

        [ObservableProperty]
        private bool _isSelected;
        [ObservableProperty]
        private string _currentStatusDisplay;
        [ObservableProperty]
        private LeadDto _leadDto;
        [ObservableProperty]
        private StatusOption _currentStatusOption;

        public bool CanConvertStage => !(LeadDto != null && LeadDto.StatusId != 3);

        public LeadItemViewModel(LeadDto leadDto, int index, ILeadService leadService, IServiceProvider serviceProvider)
        {
            _leadService = leadService;
            LeadDto = leadDto;
            CurrentStatusDisplay = leadDto.Status.ToString();
            Index = index;
            _serviceProvider = serviceProvider;
        }

        public int Index { get; set; }

        [DisplayName("ID")]
        public int Id => LeadDto.Id;
        [DisplayName("Mã khách hàng tiềm năng")]
        public string? Code => LeadDto.Code;
        [DisplayName("Tên khách hàng tiềm năng")]
        public string Name => LeadDto.Name;
        //public string? Company => LeadDto.Company;
        [DisplayName("Số điện thoại")]
        public string? Phone => LeadDto.Phone;
        [DisplayName("Email")]
        public string? Email => LeadDto.Email;
        [DisplayName("Địa chỉ")]
        public string? Address => LeadDto.Address;
        [DisplayName("Mức độ tiềm năng")]
        public string PotentialLevel => LeadDto.PotentialLevel;
        [DisplayName("Mô tả")]
        public string? Description => LeadDto.Description;
        [DisplayName("Trạng thái")]
        public string Status => LeadDto.Status;
        public int StatusId => LeadDto.StatusId;
        [DisplayName("Nhân viên phụ trách")]
        public string EmployeeName => LeadDto.EmployeeName;
        [DisplayName("Ngày bắt đầu")]
        public DateTime? StartDate => LeadDto.StartDate;
        [DisplayName("Ngày kết thúc")]
        public DateTime? EndDate => LeadDto.EndDate;

        [RelayCommand]
        private async Task ConvertStage()
        {
            //ConvertStageRequested?.Invoke();
            var convertLeadViewModel = _serviceProvider.GetRequiredService<ConvertStageViewModel>();
            convertLeadViewModel.SetLeadId(LeadDto.Id);
            var window = Window.GetWindow(System.Windows.Application.Current.MainWindow);
            var convertedLead = await ConvertDialog.ShowConvertDialog(Window.GetWindow(window), convertLeadViewModel);

            if (convertedLead)
            {
                MessageBox.Show($"Đã chuyển đổi khách hàng tiềm năng thành công.",
                    "Thành công",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        [RelayCommand]
        private async Task UpdateSelected(LeadItemViewModel leadItem)
        {
            if (leadItem == null)
            {
                MessageBox.Show("Đéo ổn");
            }
            // thông tin item để debug
            string itemInfo = leadItem.Id + " " + leadItem.Name + " " + leadItem.Email + " " + leadItem.EmployeeName;
            MessageBox.Show(itemInfo);
        }


        public async Task UpdateStatus(int newStatus)
        {
            var updateLeadRequest = new UpdateLeadStageRequest
            {
                Id = LeadDto.Id,
                StatusId = newStatus
            };
            var result = await _leadService.UpdateLeadAsync(updateLeadRequest);
            if (result.IsSuccess)
            {
                LeadDto = result.Value;
                CurrentStatusDisplay = LeadDto.Status;
                OnPropertyChanged(nameof(Status));
            }
        }

        public void RefreshProperties()
        {
            OnPropertyChanged(nameof(Id));
            OnPropertyChanged(nameof(Code));
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(Phone));
            OnPropertyChanged(nameof(Email));
            OnPropertyChanged(nameof(Address));
            OnPropertyChanged(nameof(PotentialLevel));
            OnPropertyChanged(nameof(Status));
            OnPropertyChanged(nameof(EmployeeName));
        }

    }

    public class StatusOption
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
