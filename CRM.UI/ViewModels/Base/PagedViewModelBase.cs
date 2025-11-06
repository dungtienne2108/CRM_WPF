using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.UI.ViewModels.Base
{
    public abstract partial class PagedViewModelBase<T> : ViewModelBase where T : class
    {
        private ObservableCollection<T> _items = new();
        private T _selectedItem;
        private int _currentPage = 1;
        private int _pageSize = 20;
        private int _totalPages;
        private int _totalCount;
        private string _searchText = string.Empty;
        private string _sortBy = string.Empty;
        private bool _sortAscending = true;

        public ObservableCollection<T> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }

        public T SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (SetProperty(ref _selectedItem, value))
                {
                    OnSelectedItemChanged();
                }
            }
        }

        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (SetProperty(ref _currentPage, value))
                {
                    OnPropertyChanged(nameof(CanGoToPreviousPage));
                    OnPropertyChanged(nameof(CanGoToNextPage));
                    OnPropertyChanged(nameof(PageInfo));
                }
            }
        }

        public int PageSize
        {
            get => _pageSize;
            set => SetProperty(ref _pageSize, value);
        }

        public int TotalPages
        {
            get => _totalPages;
            set
            {
                if (SetProperty(ref _totalPages, value))
                {
                    OnPropertyChanged(nameof(CanGoToNextPage));
                    OnPropertyChanged(nameof(PageInfo));
                }
            }
        }

        public int TotalCount
        {
            get => _totalCount;
            set
            {
                if (SetProperty(ref _totalCount, value))
                {
                    OnPropertyChanged(nameof(PageInfo));
                }
            }
        }

        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        public string SortBy
        {
            get => _sortBy;
            set => SetProperty(ref _sortBy, value);
        }

        public bool SortAscending
        {
            get => _sortAscending;
            set => SetProperty(ref _sortAscending, value);
        }

        public bool CanGoToPreviousPage => CurrentPage > 1;
        public bool CanGoToNextPage => CurrentPage < TotalPages;
        public string PageInfo => $"Page {CurrentPage} of {TotalPages} ({TotalCount} total)";

        [RelayCommand(CanExecute = nameof(CanGoToPreviousPage))]
        protected async Task GoToPreviousPageAsync()
        {
            if (CanGoToPreviousPage)
            {
                CurrentPage--;
                await LoadDataAsync();
            }
        }

        [RelayCommand(CanExecute = nameof(CanGoToNextPage))]
        protected async Task GoToNextPageAsync()
        {
            if (CanGoToNextPage)
            {
                CurrentPage++;
                await LoadDataAsync();
            }
        }

        [RelayCommand]
        protected async Task GoToFirstPageAsync()
        {
            CurrentPage = 1;
            await LoadDataAsync();
        }

        [RelayCommand]
        protected async Task GoToLastPageAsync()
        {
            CurrentPage = TotalPages;
            await LoadDataAsync();
        }

        [RelayCommand]
        protected async Task SearchAsync()
        {
            CurrentPage = 1;
            await LoadDataAsync();
        }

        [RelayCommand]
        protected async Task SortAsync(string column)
        {
            if (SortBy == column)
            {
                SortAscending = !SortAscending;
            }
            else
            {
                SortBy = column;
                SortAscending = true;
            }

            await LoadDataAsync();
        }

        protected abstract Task LoadDataAsync();

        protected virtual void OnSelectedItemChanged()
        {
            // Override in derived classes
        }
    }
}
