using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRM.Application.Dtos.Contract;
using CRM.Application.Interfaces;
using CRM.Application.Interfaces.Contract;
using CRM.Application.Interfaces.Deposit;
using CRM.Application.Interfaces.Payment;
using CRM.UI.ViewModels.Base;
using CRM.UI.ViewModels.PaymentManagement;
using CRM.UI.Views.ContractManagement;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Windows;

namespace CRM.UI.ViewModels.ContractManagement
{
    public partial class ContractDetailViewModel : ViewModelBase
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IContractService _contractService;
        private readonly IUploadService _uploadService;

        [ObservableProperty]
        private ContractItemViewModel _contract;
        [ObservableProperty]
        private int _contractId;
        [ObservableProperty]
        private string _contractName = string.Empty;
        [ObservableProperty]
        private string _contractCode = string.Empty;
        [ObservableProperty]
        private string _contractNumber = string.Empty;
        [ObservableProperty]
        private string _contractTypeName = string.Empty;
        [ObservableProperty]
        private int _contractTypeId;
        [ObservableProperty]
        private int _customerId;
        [ObservableProperty]
        private string _customerName = string.Empty;
        [ObservableProperty]
        private int _contractStageId;
        [ObservableProperty]
        private string _contractStageName = string.Empty;
        [ObservableProperty]
        private string? _contractDescription;
        [ObservableProperty]
        private decimal _amountBeforeTax;
        [ObservableProperty]
        private decimal _tax;
        [ObservableProperty]
        private decimal _amountAfterTax;
        [ObservableProperty]
        private decimal _amount;
        [ObservableProperty]
        private DateTime _startDate;
        [ObservableProperty]
        private DateTime _endDate;
        [ObservableProperty]
        private ObservableCollection<ContractDocumentDto> _contractDocuments = new();

        [ObservableProperty]
        private int _depositId;
        [ObservableProperty]
        private string _depositName;
        [ObservableProperty]
        private decimal _depositAmount;
        [ObservableProperty]
        private string _projectName;
        [ObservableProperty]
        private string _productName;
        [ObservableProperty]
        private decimal _productPrice;

        [ObservableProperty]
        private ObservableCollection<PaymentScheduleItemViewModel> _paymentSchedulesItems = new();
        [ObservableProperty]
        private ObservableCollection<ContractStageOption> _contractStatusOptions = new();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsDeleteVisible))]
        private bool _isEditMode = false;

        public bool IsDeleteVisible => !IsEditMode;

        #region Constructor
        public ContractDetailViewModel(
            IServiceProvider serviceProvider,
            IContractService contractService,
            IUploadService uploadService)
        {
            _serviceProvider = serviceProvider;
            _contractService = contractService;
            _uploadService = uploadService;
        }
        #endregion

        #region Public methods
        public async Task LoadDataAsync(ContractItemViewModel contractItem)
        {
            if (contractItem == null)
            {
                MessageBox.Show("Dữ liệu hợp đồng không hợp lệ.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Contract = contractItem;
            ContractId = contractItem.Id;
            await InitializeAsync();
            await GetPaymentSchedulesAsync();
        }
        #endregion

        #region Commands
        [RelayCommand]
        private void ToggleEditMode()
        {
            IsEditMode = !IsEditMode;
        }

        [RelayCommand]
        private async Task AddContractDocumentsAsync()
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Multiselect = true,
                Filter = "All Supported Files|*.pdf;*.doc;*.docx;*.xls;*.xlsx;*.png;*.jpg;*.jpeg;*.bmp;*.gif;*.txt|" +
                 "PDF Files|*.pdf|" +
                 "Word Documents|*.doc;*.docx|" +
                 "Excel Files|*.xls;*.xlsx|" +
                 "Images|*.png;*.jpg;*.jpeg;*.bmp;*.gif|" +
                 "Text Files|*.txt"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var selectedFiles = openFileDialog.FileNames;
                foreach (var filePath in selectedFiles)
                {
                    var fileData = System.IO.File.ReadAllBytes(filePath);
                    var fileName = System.IO.Path.GetFileName(filePath);
                    var uploadResult = await _uploadService.UploadFileAsync(fileData, fileName);
                    if (!string.IsNullOrEmpty(uploadResult))
                    {
                        var contractDocument = new ContractDocumentDto
                        {
                            ContractId = ContractId,
                            FileName = fileName,
                            FilePath = uploadResult,
                            ContentType = System.IO.Path.GetExtension(fileName),
                            FileSize = fileData.Length
                        };
                        ContractDocuments.Add(contractDocument);
                        // _contractService.UploadContractImage(contractImage);
                        var res = await _contractService.UploadContractImageAsync(ContractId, contractDocument);

                        if (res.IsSuccess)
                        {
                        }
                        else
                        {
                            MessageBox.Show("Thêm hình ảnh thất bại", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
        }

        [RelayCommand]
        private async Task RemoveContractDocumentAsync(ContractDocumentDto contractDocument)
        {
            if (contractDocument != null && ContractDocuments.Contains(contractDocument))
            {
                ContractDocuments.Remove(contractDocument);
                // _contractService.DeleteContractImage(contractImage.Id);
                var res = await _contractService.RemoveContractImageAsync(ContractId, contractDocument.Id);

                if (res.IsSuccess)
                {

                }
                else
                {
                    MessageBox.Show("Xóa hình ảnh thất bại", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [RelayCommand]
        private void ViewContractDocument(ContractDocumentDto contractDocument)
        {
            try
            {
                var processInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = contractDocument.FilePath,
                    UseShellExecute = true
                };

                System.Diagnostics.Process.Start(processInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể mở tài liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public bool CanSave => !HasErrors;
        [RelayCommand(CanExecute = nameof(CanSave))]
        private async Task SaveAsync()
        {
            ClearAllErrors();
            ValidateAllProperties();

            if (HasErrors)
            {
                return;
            }

            await UpdateContractAsync();
        }

        [RelayCommand]
        private async Task DeleteAsync()
        {
            try
            {
                var res = MessageBox.Show("Bạn có chắc chắn xóa hợp đồng?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (res == MessageBoxResult.Yes)
                {
                    var deleteResult = await _contractService.DeleteContractAsync(ContractId);

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
        private async Task CreatePaymentScheduleAsync()
        {
            var addPaymentScheduleViewModel = _serviceProvider.GetRequiredService<AddPaymentScheduleViewModel>();
            await addPaymentScheduleViewModel.LoadDataAsync(ContractId);
            var addPaymentSchedule = new AddPaymentScheduleDialog(addPaymentScheduleViewModel);
            var res = addPaymentSchedule.ShowDialog();

            await InitializeAsync();
            await GetPaymentSchedulesAsync();

        }
        #endregion

        #region Private Methods
        private async Task UpdateContractAsync()
        {
            try
            {
                var updateContractRequest = new UpdateContractRequest
                {
                    Id = ContractId,
                    Name = ContractName,
                    Number = ContractNumber,
                    CustomerId = CustomerId,
                    ContractStageId = ContractStageId,
                    ContractTypeId = ContractTypeId,
                    AmountBeforeTax = AmountBeforeTax,
                    Tax = Tax,
                    AmountAfterTax = AmountAfterTax,
                    Amount = Amount,
                    StartDate = StartDate,
                    EndDate = EndDate,
                    Description = ContractDescription
                };

                var result = await _contractService.UpdateContractAsync(updateContractRequest);
                if (result.IsSuccess)
                {
                    await InitializeAsync();
                    MessageBox.Show("Cập nhật hợp đồng thành công", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                else
                {
                    MessageBox.Show("Cập nhật hợp đồng thất bại", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch
            {

            }
        }

        private async Task GetContractStatusesAsync()
        {
            try
            {
                var contractStatusesResult = await _contractService.GetContractStagesAsync();
                if (contractStatusesResult.IsSuccess)
                {
                    ContractStatusOptions.Clear();
                    foreach (var status in contractStatusesResult.Value)
                    {
                        ContractStatusOptions.Add(status);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải trạng thái hợp đồng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task InitializeAsync()
        {
            IsLoading = true;

            try
            {
                var contractResult = await _contractService.GetContractByIdAsync(ContractId);
                if (contractResult.IsSuccess)
                {
                    var contract = contractResult.Value;
                    ContractId = contract.Id;
                    ContractName = contract.Name;
                    ContractCode = contract.Code;
                    ContractNumber = contract.Number;
                    ContractTypeId = contract.TypeId;
                    ContractTypeName = contract.Type;
                    CustomerId = contract.CustomerId;
                    CustomerName = contract.CustomerName;
                    ContractStageId = contract.StatusId;
                    ContractStageName = contract.Status;
                    //DepositAmount = contract.
                    ContractDescription = contract.Description;
                    AmountBeforeTax = contract.AmountBeforeTax;
                    Tax = contract.Tax;
                    AmountAfterTax = contract.AmountAfterTax;
                    Amount = contract.Amount;
                    StartDate = contract.StartDate;
                    EndDate = contract.EndDate;
                    DepositId = contract.DepositId;
                    ContractDocuments = new ObservableCollection<ContractDocumentDto>(contract.Documents);
                }
                else
                {
                    MessageBox.Show($"Lỗi khi tải hợp đồng : {contractResult.Error.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var depositService = _serviceProvider.GetRequiredService<IDepositService>();
                var depositResult = await depositService.GetDepositByIdAsync(DepositId);
                if (depositResult.IsSuccess)
                {
                    var deposit = depositResult.Value;
                    DepositName = deposit.Name;
                    DepositAmount = deposit.Amount;
                    ProductName = deposit.ProductName;
                    ProductPrice = deposit.ProductPrice;
                    ProjectName = deposit.ProjectName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu hợp đồng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task GetPaymentSchedulesAsync()
        {
            IsLoading = true;

            try
            {
                var paymentService = _serviceProvider.GetRequiredService<IPaymentService>();
                var paymentSchedulesResult = await paymentService.GetPaymentSchedulesByContractIdAsync(ContractId);

                if (paymentSchedulesResult.IsSuccess)
                {
                    PaymentSchedulesItems.Clear();
                    int index = 1;
                    foreach (var paymentSchedule in paymentSchedulesResult.Value)
                    {
                        PaymentSchedulesItems.Add(new(paymentSchedule, index++));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải lịch trình thanh toán: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }
        #endregion

        #region Property changed
        partial void OnIsEditModeChanged(bool value)
        {
            if (value)
            {
                _ = GetContractStatusesAsync();
            }
        }

        partial void OnAmountBeforeTaxChanged(decimal value)
        {
            AmountAfterTax = AmountBeforeTax + (AmountBeforeTax * Tax / 100);
            Amount = AmountAfterTax - DepositAmount;
        }

        partial void OnTaxChanged(decimal value)
        {
            AmountAfterTax = AmountBeforeTax + (AmountBeforeTax * Tax / 100);
            Amount = AmountAfterTax - DepositAmount;
        }
        #endregion
    }
}
