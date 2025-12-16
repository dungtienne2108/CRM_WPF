using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRM.Application.Dtos.Employee;
using CRM.Application.Dtos.Lead;
using CRM.Application.Dtos.Opportunity;
using CRM.Application.Dtos.Project;
using CRM.Application.Interfaces.Opportunity;
using CRM.Application.Interfaces.Project;
using CRM.UI.ViewModels.Base;
using CRM.UI.ViewModels.DepositManagement;
using CRM.UI.ViewModels.LeadManagement;
using CRM.UI.Views.DepositManagement;
using CRM.UI.Views.OpportunityManagement;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;

namespace CRM.UI.ViewModels.OpportunityManagement
{
    public partial class OpportunityDetailViewModel : ViewModelBase
    {
        private readonly IOpportunityService _opportunityService;
        private readonly IProjectService _projectService;
        private readonly IServiceProvider _serviceProvider;

        [ObservableProperty]
        private string _status = "Đàm phán";
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsStatusStepsVisible))]
        [NotifyPropertyChangedFor(nameof(IsConvertStageVisible))]
        [NotifyPropertyChangedFor(nameof(CanDeposit))]
        [NotifyCanExecuteChangedFor(nameof(PreviousStatusCommand))]
        [NotifyCanExecuteChangedFor(nameof(NextStatusCommand))]
        private int _currentStatusIndex;
        [ObservableProperty]
        private OpportunityItemViewModel _opportunity;

        public bool CanDeposit => CurrentStatusIndex == 3;

        [ObservableProperty]
        private int _opportunityId;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Tên cơ hội bán hàng không được để trống.")]
        [MinLength(3, ErrorMessage = "Tên cơ hội bán hàng không được ít hơn 3 ký tự.")]
        private string _opportunityName;
        [ObservableProperty]
        private string _opportunityCode;
        [ObservableProperty]
        private DateTime? _startDate;
        [ObservableProperty]
        private DateTime? _endDate;
        [ObservableProperty]
        private string _customerName;
        [ObservableProperty]
        private string _customerEmail;
        [ObservableProperty]
        private string _customerPhone;
        [ObservableProperty]
        private string _customerAddress;
        [ObservableProperty]
        private string _description;
        [ObservableProperty]
        private string _employeeName;
        [ObservableProperty]
        private ObservableCollection<ProductDto> _products;
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(DeleteProductCommand))]
        private ProductDto? _selectedProduct;
        [ObservableProperty]
        private bool _isSelectedProduct = false;

        [ObservableProperty]
        private ObservableCollection<OpportunityStatusOption> _statusOptions;
        [ObservableProperty]
        private ObservableCollection<ProductStatusOption> _productStatusOptions;
        [ObservableProperty]
        private int _selectedStatusOptionId;

        [ObservableProperty]
        private bool _isEditMode = false;
        [ObservableProperty]
        private bool _isChangingStatus;

        public bool IsDeleteVisible => !IsEditMode;
        public bool IsStatusStepsVisible => true;
        public bool IsConvertStageVisible => CurrentStatusIndex == 2;


        public EmployeeDto Employee => Opportunity.Employee;


        public OpportunityDetailViewModel(
            IOpportunityService opportunityService,
            IProjectService projectService,
            IServiceProvider serviceProvider)
        {
            _opportunityService = opportunityService;
            _projectService = projectService;
            _serviceProvider = serviceProvider;
        }

        public ObservableCollection<StatusStep> StatusSteps { get; set; }
        public ObservableCollection<RelatedStaff> RelatedStaff { get; set; }

        public async Task LoadDataAsync(OpportunityItemViewModel opportunity)
        {
            if (opportunity == null)
            {
                return;
            }

            Opportunity = opportunity;

            await LoadOpportunityAsync();
            await LoadOpportunityStatusAsync();
            await LoadProductStatusesAsync();
            await InitializeStatusOptions();
        }
        #region Private Methods

        private async Task LoadOpportunityAsync()
        {
            var opportunityResult = await _opportunityService.GetOpportunityByIdAsync(Opportunity.OpportunityId);
            if (opportunityResult.IsSuccess)
            {
                Opportunity = new OpportunityItemViewModel(Opportunity.Index, opportunityResult.Value);
                OpportunityId = Opportunity.OpportunityId;
                OpportunityName = Opportunity.OpportunityName;
                OpportunityCode = Opportunity.OpportunityCode;
                StartDate = Opportunity.CreateDate;
                EndDate = Opportunity.EndDate.ToDateTime(new TimeOnly(0, 0));
                CustomerName = Opportunity.Customer.Name;
                CustomerEmail = Opportunity.Customer.Email;
                CustomerPhone = Opportunity.Customer.Phone;
                CustomerAddress = Opportunity.Customer.Address;
                Description = Opportunity.OpportunityDescription;
                EmployeeName = Opportunity.Employee.Name;
                Status = Opportunity.OpportunityStatus.Name;
                Products = new ObservableCollection<ProductDto>(Opportunity.Products);
                UpdateStatusSteps();
            }
        }

        private async Task InitializeStatusOptions()
        {
            var statusOptions = await _opportunityService.GetAllOpportunityStatusesAsync();

            // convert to ObservableCollection
            StatusSteps = new ObservableCollection<StatusStep>(
                statusOptions.Select((status, index) => new StatusStep
                {
                    Title = status.Name,
                    Subtitle = "",
                    Index = status.Id - 1,
                    IsLast = index == statusOptions.Count() - 1,
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

            CurrentStatusIndex = Opportunity.OpportunityStatus.Id - 1;
            Status = Opportunity.OpportunityStatus.Name;

            for (int i = 0; i < StatusSteps.Count; i++)
            {
                StatusSteps[i].IsCompleted = i <= CurrentStatusIndex;
            }
        }
        private async Task UpdateStatusFromIndex()
        {
            IsChangingStatus = true;

            var res = await _opportunityService.UpdateOpportunityStageAsync(Opportunity.OpportunityId, CurrentStatusIndex + 1);

            if (res.IsSuccess)
            {
                Opportunity = new OpportunityItemViewModel(Opportunity.Index, res.Value);
                Status = Opportunity.OpportunityStatus.Name;
                UpdateStatusSteps();
            }

            IsChangingStatus = false;
        }

        private async Task LoadOpportunityStatusAsync()
        {
            var statusOptions = await _opportunityService.GetAllOpportunityStatusesAsync();
            StatusOptions = new ObservableCollection<OpportunityStatusOption>(statusOptions);
        }
        #endregion
        #region Commands

        [RelayCommand]
        private void ToggleEditMode()
        {
            IsEditMode = !IsEditMode;
        }

        [RelayCommand]
        private async Task NewDepositAsync()
        {
            if (Opportunity == null)
            {
                MessageBox.Show("Cơ hội bán hàng không hợp lệ.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!CanDeposit)
            {
                MessageBox.Show("Cơ hội bán hàng chưa đạt trạng thái 'Chốt'.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var depositViewModel = _serviceProvider.GetRequiredService<AddDepositViewModel>();
            await depositViewModel.InitializeAsync(Opportunity);

            var depositView = new AddDepositDialog(depositViewModel);
            depositView.ShowDialog();
        }

        [RelayCommand]
        private async Task DeleteAsync()
        {
            try
            {
                var res = MessageBox.Show("Bạn có chắc chắn xóa cơ hội bán hàng ?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (res == MessageBoxResult.Yes)
                {
                    var deleteResult = await _opportunityService.DeleteOpportunityAsync(OpportunityId);

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
        private async Task UpdateOpportunityAsync()
        {
            ClearAllErrors();
            ValidateAllProperties();
            if (HasErrors)
            {
                return;
            }
            var updateRequest = new UpdateOpportunityRequest
            {
                OpportunityId = Opportunity.OpportunityId,
                OpportunityName = OpportunityName,
                StartDate = StartDate,
                EndDate = EndDate,
                Description = Description
            };
            var result = await _opportunityService.UpdateOpportunityAsync(updateRequest);
            if (result.IsSuccess)
            {
                Opportunity = new OpportunityItemViewModel(Opportunity.Index, result.Value);
                await LoadOpportunityAsync();
                UpdateStatusSteps();
                IsEditMode = false;

                MessageBox.Show("Cập nhật cơ hội bán hàng thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                SetError(result.Error);
                MessageBox.Show(result.Error, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanMoveToPreviousStatus => CurrentStatusIndex > 0;

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
        private async Task AddProductAsync()
        {
            var addProductToOpportunityViewModel = _serviceProvider.GetRequiredService<AddProductToOpportunityViewModel>();
            await addProductToOpportunityViewModel.LoadDataAsync(OpportunityId);
            var addProductToOpportunityDialog = new AddProductToOpportunityDialog(addProductToOpportunityViewModel);
            addProductToOpportunityDialog.ShowDialog();
            await LoadOpportunityAsync();
        }

        [RelayCommand(CanExecute = nameof(CanDelete))]
        private async Task DeleteProductAsync()
        {
            if (SelectedProduct == null)
            {
                return;
            }
            var result = MessageBox.Show($"Bạn có chắc chắn xóa sản phẩm '{SelectedProduct.ProductName}' khỏi cơ hội bán hàng?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                var deleteResult = await _opportunityService.RemoveProductFromOpportunityAsync(OpportunityId, SelectedProduct.ProductId);
                if (deleteResult.IsSuccess)
                {
                    MessageBox.Show("Xóa sản phẩm thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    SelectedProduct = null;
                    Products.Remove(SelectedProduct);
                    await LoadOpportunityAsync();
                }
                else
                {
                    MessageBox.Show("Xóa sản phẩm thất bại.", "Thất bại", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async Task LoadProductStatusesAsync()
        {
            var result = await _projectService.GetProductStatusesAsync();
            if (result.IsSuccess)
            {
                ProductStatusOptions = new ObservableCollection<ProductStatusOption>(result.Value);
            }
            else
            {
                ProductStatusOptions = new ObservableCollection<ProductStatusOption>();
            }
        }

        private async Task UpdateProductStatusAsync(int productId, int newStatusId)
        {
            try
            {
                if (SelectedProduct != null && SelectedProduct.ProductStatusId == newStatusId)
                {
                    MessageBox.Show("Trạng thái sản phẩm không thay đổi.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var result = await _projectService.UpdateProductStatusAsync(OpportunityId, productId, newStatusId);
                if (result.IsSuccess)
                {
                    await LoadOpportunityAsync();
                    MessageBox.Show("Cập nhật trạng thái sản phẩm thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show($"{result.Error.Message}", "Thất bại", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception)
            {
                MessageBox.Show($"Cập nhật trạng thái sản phẩm thất bại. Vui lòng kiểm tra lại.", "Thất bại", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public bool CanDelete() => SelectedProduct != null;
        #endregion

        #region Property changes
        partial void OnSelectedProductChanged(ProductDto? value)
        {
            if (value != null)
            {
                IsSelectedProduct = true;
            }
            else
            {
                IsSelectedProduct = false;
            }
        }

        partial void OnSelectedStatusOptionIdChanged(int value)
        {
            if (SelectedProduct != null)
            {
                _ = UpdateProductStatusAsync(SelectedProduct.ProductId, value);
            }
        }
        #endregion
    }
}
