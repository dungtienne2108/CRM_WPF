using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRM.Application.Dtos.Lead;
using CRM.Application.Interfaces.Leads;
using CRM.UI.ViewModels.Base;
using CRM.UI.Views.LeadManagement;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;

namespace CRM.UI.ViewModels.LeadManagement
{
    public partial class LeadDetailViewModel : ViewModelBase
    {
        private readonly ILeadService _leadService;
        private readonly IServiceProvider _serviceProvider;

        [ObservableProperty]
        private string _status = "Mở";
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsSaveEnabled))]
        [NotifyPropertyChangedFor(nameof(IsStatusStepsVisible))]
        [NotifyPropertyChangedFor(nameof(IsConvertStageVisible))]
        [NotifyCanExecuteChangedFor(nameof(PreviousStatusCommand))]
        [NotifyCanExecuteChangedFor(nameof(NextStatusCommand))]
        private int _currentStatusIndex;

        [ObservableProperty]
        private int _leadId;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Tên khách hàng tiềm năng không được để trống.")]
        [MinLength(3, ErrorMessage = "Tên khách hàng tiềm năng phải có ít nhất 3 ký tự.")]
        private string _leadName;
        [ObservableProperty]
        private string _leadCode;
        [ObservableProperty]
        private string _company;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Số điện thoại không được để trống.")]
        private string _phone;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
        private string _email;
        [ObservableProperty]
        private string _address;
        [ObservableProperty]
        private string _potentialLevel;
        [ObservableProperty]
        private int _potentialLevelId;
        [ObservableProperty]
        private string _description;
        [ObservableProperty]
        private string _employeeName;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Ngày bắt đầu không được để trống.")]
        private DateTime? _startDate;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Ngày kết thúc không được để trống.")]
        private DateTime? _endDate;

        [ObservableProperty]
        private ObservableCollection<LeadPotentialLevelDto> _potentialLevelOptions;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsStatusStepsVisible))]
        private bool _isChangingStatus;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsDeleteVisible))]
        private bool _isEditMode = false;

        public bool IsSaveEnabled => !HasErrors;
        public bool IsDeleteVisible => !IsEditMode;
        public bool IsStatusStepsVisible => true;
        public bool IsConvertStageVisible => CurrentStatusIndex == 2;

        [ObservableProperty]
        private LeadItemViewModel _lead;

        public LeadDetailViewModel(ILeadService leadService, IServiceProvider serviceProvider)
        {
            _leadService = leadService;
            _serviceProvider = serviceProvider;

            ErrorsChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(IsSaveEnabled));
            };
        }


        public ObservableCollection<StatusStep> StatusSteps { get; set; }
        public ObservableCollection<RelatedStaff> RelatedStaff { get; set; }

        #region public methods
        public async Task LoadDataAsync(LeadItemViewModel leadItem)
        {
            if (leadItem == null)
            {
                return;
            }

            Lead = leadItem;
            await LoadLeadAsync();
            await LoadPotentialLevelOptions();
            await InitializeStatusSteps();
        }
        #endregion

        #region Private Methods
        private async Task InitializeStatusSteps()
        {
            var leadStages = await _leadService.GetAllLeadStagesAsync();

            StatusSteps = new ObservableCollection<StatusStep>(
                leadStages.Select((stage, index) => new StatusStep
                {
                    Title = stage.Name,
                    Subtitle = "",
                    Index = stage.Id - 1,
                    IsLast = index == leadStages.Count() - 1,
                })
            );

            UpdateStatusSteps();
        }

        private void UpdateStatusSteps()
        {
            if (StatusSteps == null || StatusSteps.Count == 0)
            {
                return;
            }

            CurrentStatusIndex = Lead.StatusId - 1;
            Status = Lead.Status;

            for (int i = 0; i < StatusSteps.Count; i++)
            {
                StatusSteps[i].IsCompleted = i <= CurrentStatusIndex;
            }
        }
        private async Task UpdateStatusFromIndex()
        {
            IsChangingStatus = true;

            var res = await _leadService.UpdateLeadStageAsync(Lead.Id, CurrentStatusIndex + 1);

            if (res.IsSuccess)
            {
                Lead = new LeadItemViewModel(res.Value, Lead.Index, _leadService, _serviceProvider);
                Status = Lead.Status;
                UpdateStatusSteps();
            }

            IsChangingStatus = false;
        }

        private async Task LoadLeadAsync()
        {
            var leadResult = await _leadService.GetLeadByIdAsync(Lead.Id);
            if (leadResult.IsSuccess)
            {
                Lead = new LeadItemViewModel(leadResult.Value, Lead.Index, _leadService, _serviceProvider);
                LeadId = Lead.Id;
                LeadName = Lead.Name ?? string.Empty;
                LeadCode = Lead.Code ?? string.Empty;
                Phone = Lead.Phone ?? string.Empty;
                Email = Lead.Email ?? string.Empty;
                Address = Lead.Address ?? string.Empty;
                PotentialLevel = Lead.PotentialLevel;
                Description = Lead.Description ?? string.Empty;
                EmployeeName = Lead.EmployeeName;
                Status = Lead.Status;
                StartDate = Lead.StartDate;
                EndDate = Lead.EndDate;
                UpdateStatusSteps();
            }
        }

        private async Task LoadPotentialLevelOptions()
        {
            var potentialLevels = await _leadService.GetAllLeadPotentialLevelsAsync();
            PotentialLevelOptions = new ObservableCollection<LeadPotentialLevelDto>(potentialLevels);
        }

        private bool ValidateUpdateLead()
        {
            if (string.IsNullOrWhiteSpace(LeadName))
            {
                SetError("Tên khách hàng tiềm năng không được để trống.");
                return false;
            }

            if (LeadName.Length < 3)
            {
                SetError("Tên khách hàng tiềm năng phải có ít nhất 3 ký tự.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(Phone))
            {
                SetError("Số điện thoại không được để trống.");
                return false;
            }

            if (!string.IsNullOrWhiteSpace(Email) && !new EmailAddressAttribute().IsValid(Email))
            {
                SetError("Địa chỉ email không hợp lệ.");
                return false;
            }

            return true;
        }
        #endregion

        #region Commands

        [RelayCommand]
        private void ToggleEditMode()
        {
            IsEditMode = !IsEditMode;
        }

        [RelayCommand]
        private async Task DeleteAsync()
        {
            try
            {
                var res = MessageBox.Show("Bạn có chắc chắn xóa khách hàng tiềm năng ?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (res == MessageBoxResult.Yes)
                {
                    var deleteResult = await _leadService.DeleteLeadAsync(LeadId);

                    if (deleteResult.IsSuccess)
                    {
                        MessageBox.Show("Xóa thành công", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                        System.Windows.Application.Current.Windows
                        .OfType<Window>()
                        .SingleOrDefault(w => w.DataContext == this)?
                        .Close();
                    }
                    else
                    {
                        MessageBox.Show("Xóa thất bại", "Thấy bại", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch { MessageBox.Show("Xóa thất bại", "Thấy bại", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        [RelayCommand]
        private async Task UpdateLead()
        {
            ClearAllErrors();
            ValidateAllProperties();
            if (HasCustomErrors)
            {
                MessageBox.Show("Vui lòng kiểm tra lại thông tin khách hàng tiềm năng.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!ValidateUpdateLead())
            {
                MessageBox.Show(ErrorMessage, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                var updateLeadRequest = new UpdateLeadRequest
                {
                    Id = LeadId,
                    Code = LeadCode,
                    Name = LeadName,
                    Phone = Phone,
                    Email = Email,
                    Address = Address,
                    PotentialLevelId = PotentialLevelId,
                    Description = Description,
                    StatusId = CurrentStatusIndex + 1,
                    StartDate = StartDate.Value,
                    EndDate = EndDate.Value
                };

                var result = await _leadService.UpdateLeadAsync(updateLeadRequest);

                if (result.IsSuccess)
                {
                    Lead = new LeadItemViewModel(result.Value, Lead.Index, _leadService, _serviceProvider);
                    Status = Lead.Status;
                    UpdateStatusSteps();
                    IsEditMode = false;

                    MessageBox.Show("Cập nhật thông tin khách hàng tiềm năng thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    SetError(result.Error);
                    MessageBox.Show($"Cập nhật thông tin khách hàng tiềm năng thất bại: {result.Error.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception)
            {
            }
        }

        [RelayCommand]
        private async Task ConvertStageAsync()
        {
            var convertLeadViewModel = _serviceProvider.GetRequiredService<ConvertStageViewModel>();
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

        private bool CanMoveToPreviousStatus => CurrentStatusIndex > 0 && CurrentStatusIndex < 2;

        [RelayCommand(CanExecute = nameof(CanMoveToPreviousStatus))]
        private async Task PreviousStatus(object parameter)
        {
            if (CurrentStatusIndex > 0)
            {
                IsChangingStatus = true;
                CurrentStatusIndex--;
                await UpdateStatusFromIndex();
                IsChangingStatus = false;
            }
        }

        private bool CanMoveToNextStatus => CurrentStatusIndex < StatusSteps.Count - 1;

        [RelayCommand(CanExecute = nameof(CanMoveToNextStatus))]
        private async Task NextStatus(object parameter)
        {
            if (CurrentStatusIndex < StatusSteps.Count - 1)
            {
                IsChangingStatus = true;
                CurrentStatusIndex++;
                await UpdateStatusFromIndex();
                IsChangingStatus = false;
            }
        }

        [RelayCommand]
        private async Task SelectStatus(StatusStep step)
        {
            if (step != null && step.Index != CurrentStatusIndex + 1)
            {
                CurrentStatusIndex = step.Index - 1;
                await UpdateStatusFromIndex();
            }
        }

        #endregion

    }

    public class RelatedStaff
    {
        public required string Name { get; set; }
        public required string Role { get; set; }
        public required string Description { get; set; }
        public required string Initial { get; set; }
    }

    public partial class StatusStep : ViewModelBase
    {
        [ObservableProperty]
        private bool _isCompleted;

        public new required string Title { get; set; }
        public required string Subtitle { get; set; }
        public int Index { get; set; }
        public bool IsLast { get; set; }
        public bool IsFirst => Index == 0;
    }
}
