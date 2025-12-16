using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRM.Application.Dtos.Contact;
using CRM.Application.Dtos.Deposit;
using CRM.Application.Interfaces.Contact;
using CRM.Application.Interfaces.Deposit;
using CRM.Application.Interfaces.Project;
using CRM.UI.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows;

namespace CRM.UI.ViewModels.DepositManagement
{
    public partial class DepositDetailViewModel : ViewModelBase
    {
        private readonly IDepositService _depositService;
        private readonly IProjectService _projectService;
        private readonly IContactService _contactService;

        [ObservableProperty]
        private DepositItemViewModel _deposit;

        [ObservableProperty]
        private int _depositId;
        [ObservableProperty]
        private string _depositName = string.Empty;
        [ObservableProperty]
        private string _depositCode = string.Empty;

        [ObservableProperty]
        private string _opportunityName = string.Empty;
        [ObservableProperty]
        private int _customerId;
        [ObservableProperty]
        private string _customerName = string.Empty;
        [ObservableProperty]
        private string _employeeName = string.Empty;
        [ObservableProperty]
        private decimal _amount;
        [ObservableProperty]
        private DateTime _startDate;
        [ObservableProperty]
        private DateTime _endDate;
        [ObservableProperty]
        private string _projectName = string.Empty;
        [ObservableProperty]
        private string _productName = string.Empty;
        [ObservableProperty]
        private decimal _productPrice;
        [ObservableProperty]
        private string? _description = string.Empty;

        [ObservableProperty]
        private int? _contactId;
        [ObservableProperty]
        private string _contactName = string.Empty;
        [ObservableProperty]
        private string _contactPhone = string.Empty;
        [ObservableProperty]
        private string _contactEmail = string.Empty;
        [ObservableProperty]
        private string? _contactDescription = string.Empty;
        [ObservableProperty]
        private string? _contactType = string.Empty;

        [ObservableProperty]
        private ObservableCollection<ContactDto> _contactOptions = new();
        [ObservableProperty]
        private ContactDto? _selectedContact;
        [ObservableProperty]
        private string _contactSearchKeyword = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsDeleteVisible))]
        private bool _isEditMode = false;

        public bool IsDeleteVisible => !IsEditMode;

        public DepositDetailViewModel(
            IDepositService depositService,
            IProjectService projectService,
            IContactService contactService)
        {
            _depositService = depositService;
            _projectService = projectService;
            _contactService = contactService;
        }

        #region Public Methods
        public async Task LoadDataAsync(DepositItemViewModel depositItem)
        {
            if (depositItem == null || depositItem.Id <= 0)
            {
                return;
            }

            Deposit = depositItem;
            DepositId = depositItem.Id;

            await GetDepositAsync();
        }
        #endregion

        #region Commands
        public bool CanSave => !HasErrors;
        [RelayCommand(CanExecute = nameof(CanSave))]
        private void ToggleEditMode()
        {
            IsEditMode = !IsEditMode;
        }

        [RelayCommand]
        private async Task DeleteAsync()
        {
            try
            {
                var res = MessageBox.Show("Bạn có chắc chắn xóa đặt cọc?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (res == MessageBoxResult.Yes)
                {
                    var deleteResult = await _depositService.DeleteDepositAsync(DepositId);

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
        private async Task SaveAsync()
        {
            ClearAllErrors();
            ValidateAllProperties();

            await UpdateDepositAsync();
        }
        #endregion

        #region Private Methods
        private async Task UpdateDepositAsync()
        {
            if (!IsEditMode)
            {
                MessageBox.Show("Không cho phép chỉnh sửa.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var updateRequest = new UpdateDepositRequest
                {
                    DepositId = DepositId,
                    DepositName = DepositName,
                    Amount = Amount,
                    StartDate = StartDate,
                    EndDate = EndDate,
                    Description = Description,
                    ContactId = ContactId
                };
                var updateResult = await _depositService.UpdateDepositAsync(updateRequest);
                if (updateResult.IsFailure)
                {
                    MessageBox.Show($"Cập nhật đặt cọc thất bại: {updateResult.Error.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                MessageBox.Show("Cập nhật đặt cọc thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                IsEditMode = false;
                await GetDepositAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Cập nhật đặt cọc thất bại: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task GetContactsAsync()
        {
            try
            {
                var getContactRequest = new GetContactRequest
                {
                    PageNumber = 1,
                    PageSize = 1000
                };

                var contactsResult = await _contactService.GetContactsByCustomerIdAsync(CustomerId);

                ContactOptions.Clear();

                if (contactsResult.Count() == 0)
                {
                    return;
                }

                foreach (var contact in contactsResult)
                {
                    ContactOptions.Add(contact);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lấy danh sách liên hệ thất bại: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task GetDepositAsync()
        {
            var depositResult = await _depositService.GetDepositByIdAsync(DepositId);
            if (depositResult.IsFailure)
            {
                MessageBox.Show($"Lấy thông tin đặt cọc thất bại: {depositResult.Error.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var deposit = depositResult.Value;

            if (deposit == null)
            {
                MessageBox.Show("Không tìm thấy thông tin đặt cọc.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //var productsResult = await _projectService.GetProductByOpportunityIdAsync(deposit.OpportunityId);
            //if (productsResult.IsSuccess)
            //{
            //    var product = productsResult.Value.FirstOrDefault();
            //    if (product != null)
            //    {
            //        ProjectName = product.ProjectName;
            //        ProductName = product.ProductName;
            //        ProductPrice = product.ProductPrice.HasValue ? product.ProductPrice.Value : 0;
            //    }
            //}
            ProductName = deposit.ProductName;
            ProductPrice = deposit.ProductPrice;
            ProjectName = deposit.ProjectName;

            if (deposit.ContactId != null)
            {
                var contactResult = await _contactService.GetContactByIdAsync(deposit.ContactId.Value);
                if (contactResult.IsSuccess)
                {
                    var contact = contactResult.Value;
                    ContactSearchKeyword = contact.Name;
                    ContactName = contact.Name;
                    ContactPhone = contact.Phone ?? string.Empty;
                    ContactEmail = contact.Email ?? string.Empty;
                    ContactDescription = contact.Description ?? string.Empty;
                    ContactType = contact.ContactType;
                }
                else
                {
                    ContactName = string.Empty;
                    ContactPhone = string.Empty;
                    ContactEmail = string.Empty;
                    ContactDescription = string.Empty;
                    ContactType = string.Empty;
                }
            }

            DepositName = deposit.Name;
            DepositCode = deposit.Code;
            OpportunityName = deposit.OpportunityName;
            CustomerId = deposit.CustomerId;
            CustomerName = deposit.CustomerName;
            EmployeeName = deposit.EmployeeName;
            Amount = deposit.Amount;
            StartDate = deposit.StartDate;
            EndDate = deposit.EndDate;
            Description = deposit.Description;
        }
        #endregion

        #region Property changed
        partial void OnIsEditModeChanged(bool value)
        {
            if (value)
            {
                _ = GetContactsAsync();
            }
        }

        partial void OnSelectedContactChanged(ContactDto? value)
        {
            if (value != null)
            {
                ContactId = value.Id;
                ContactName = value.Name;
                ContactPhone = value.Phone ?? string.Empty;
                ContactEmail = value.Email ?? string.Empty;
                ContactDescription = value.Description ?? string.Empty;
            }
        }

        partial void OnContactSearchKeywordChanged(string value)
        {
            if (SelectedContact != null && SelectedContact.Name != value)
            {
                _ = GetContactsAsync();
            }
        }
        #endregion
    }
}
