using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CRM.Application.Dtos.Contact;
using CRM.Application.Interfaces.Contact;
using CRM.Infrastructure.Services;
using CRM.UI.ViewModels.Base;
using CRM.UI.Views.ContactManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows;

namespace CRM.UI.ViewModels.ContactManagement
{
    public partial class ContactManagementViewModel : ViewModelBase
    {
        private readonly IContactService _contactService;
        private readonly IServiceProvider _serviceProvider;

        [ObservableProperty]
        private string searchText = string.Empty;
        [ObservableProperty]
        private int currentPage;
        [ObservableProperty]
        private int recordsPerPage;
        [ObservableProperty]
        private int totalRecords;
        [ObservableProperty]
        private string totalRecordsText = string.Empty;
        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private ObservableCollection<ContactItemViewModel> _contactItems = new();
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(OpenDetailCommand))]
        private ContactItemViewModel? _selectedContactItem;

        public ContactManagementViewModel(IContactService contactService, IServiceProvider serviceProvider)
        {
            _contactService = contactService;
            _serviceProvider = serviceProvider;

            CurrentPage = 1;
            RecordsPerPage = 10;
            TotalRecords = 0;
            TotalRecordsText = "Tổng số bản ghi: 0";
            IsLoading = false;
        }


        public static readonly List<int> RecordsPerPageOptions = new() { 10, 25, 50, 100 };

        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / RecordsPerPage);

        #region Commands
        [RelayCommand]
        private async Task SearchAsync()
        {
            CurrentPage = 1;
            await InitializeAsync();
        }

        [RelayCommand]
        private async Task CreateNewAsync()
        {
            var addContactViewModel = _serviceProvider.GetRequiredService<AddContactViewModel>();
            await addContactViewModel.LoadDataAsync();

            var addContactDialog = new AddContactDialog(addContactViewModel);
            var res = addContactDialog.ShowDialog();

            await InitializeAsync();
            //if (res == true)
            //{
            //    await InitializeAsync();
            //}
        }

        [RelayCommand]
        private async Task Export()
        {
            var dialog = new SaveFileDialog
            {
                Title = "Xuất danh sách liên hệ",
                Filter = "Excel Workbook (*.xlsx)|*.xlsx",
                FileName = "ContactList.xlsx",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (dialog.ShowDialog() == true)
            {
                var exportPath = dialog.FileName;
                var exportListResult = await _contactService.GetContactsAsync(new()
                {
                    PageNumber = 1,
                    PageSize = int.MaxValue
                });

                ExcelHelper.ExportToExcelFile(exportListResult.Items, exportPath, "Contacts");

                MessageBox.Show("Xuất file Excel thành công!", "Thành công",
            MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        [RelayCommand]
        private async Task ChangeRecordsPerPage(int newRecordsPerPage)
        {
            RecordsPerPage = newRecordsPerPage;
            CurrentPage = 1;
            await InitializeAsync();
        }

        // Pagination
        [RelayCommand(CanExecute = nameof(CanExecuteFirstPage))]
        private async Task FirstPage()
        {
            CurrentPage = 1;
            await InitializeAsync();
        }
        private bool CanExecuteFirstPage()
        {
            return CurrentPage > 1;
        }

        [RelayCommand(CanExecute = nameof(CanExecutePreviousPage))]
        private async Task PreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                await InitializeAsync();
            }
        }
        private bool CanExecutePreviousPage()
        {
            return CurrentPage > 1;
        }

        [RelayCommand(CanExecute = nameof(CanExecuteNextPage))]
        private async Task NextPage()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                await InitializeAsync();
            }
        }
        private bool CanExecuteNextPage()
        {
            return CurrentPage < TotalPages;
        }

        [RelayCommand(CanExecute = nameof(CanExecuteLastPage))]
        private async Task LastPage()
        {
            CurrentPage = TotalPages;
            await InitializeAsync();
        }
        private bool CanExecuteLastPage()
        {
            return CurrentPage < TotalPages;
        }

        [RelayCommand]
        private async Task GoToPage(int pageNumber)
        {
            if (pageNumber >= 1 && pageNumber <= TotalPages)
            {
                CurrentPage = pageNumber;
                await InitializeAsync();
            }
            else
            {
                CurrentPage = 1;
                await InitializeAsync();
            }
        }

        [RelayCommand]
        private async Task OpenDetailAsync(ContactItemViewModel contactItem)
        {
            var detailViewModel = _serviceProvider.GetRequiredService<ContactDetailViewModel>();
            await detailViewModel.LoadDataAsync(contactItem.Id);
            var detailDialog = new ContactDetailView(detailViewModel);
            detailDialog.ShowDialog();
            await InitializeAsync();
        }
        #endregion

        #region Initialization
        public async Task InitializeAsync()
        {
            IsLoading = true;
            try
            {
                var getContactRequest = new GetContactRequest
                {
                    Keyword = SearchText,
                    PageNumber = CurrentPage,
                    PageSize = RecordsPerPage
                };

                var contacts = await _contactService.GetContactsAsync(getContactRequest);

                ContactItems.Clear();
                int index = (CurrentPage - 1) * RecordsPerPage + 1;

                foreach (var contact in contacts.Items)
                {
                    ContactItems.Add(new ContactItemViewModel(contact, index++));
                }

                TotalRecords = contacts.TotalCount;
                TotalRecordsText = $"Tổng số: {TotalRecords} bản ghi";
            }
            catch (Exception)
            {
            }
            finally
            {
                IsLoading = false;
            }
        }
        #endregion
    }
}
