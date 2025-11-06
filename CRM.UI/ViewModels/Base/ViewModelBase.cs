using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CRM.UI.ViewModels.Base
{
    public abstract partial class ViewModelBase : ObservableValidator, IDisposable
    {
        private bool _isBusy;
        private string _title = string.Empty;
        private bool _isInitialized;
        private bool _hasCustomErrors;
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private bool isLoading;

        protected ViewModelBase()
        {
            ErrorsChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(HasAnyErrors));
            };
        }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (SetProperty(ref _isBusy, value))
                {
                    OnPropertyChanged(nameof(IsNotBusy));
                }
            }
        }

        public bool IsNotBusy => !IsBusy;

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public bool IsInitialized
        {
            get => _isInitialized;
            set => SetProperty(ref _isInitialized, value);
        }

        public bool HasCustomErrors
        {
            get => _hasCustomErrors;
            set => SetProperty(ref _hasCustomErrors, value);
        }

        public bool HasAnyErrors => HasErrors || HasCustomErrors;

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                if (SetProperty(ref _errorMessage, value))
                {
                    HasCustomErrors = !string.IsNullOrEmpty(value);
                    OnPropertyChanged(nameof(HasAnyErrors));
                }
            }
        }

        public virtual Task InitializeAsync(object parameter = null)
        {
            return Task.CompletedTask;
        }

        public virtual Task RefreshAsync()
        {
            return Task.CompletedTask;
        }

        protected virtual void OnNavigatedTo(object parameter)
        {
        }

        protected virtual void OnNavigatedFrom()
        {
        }

        public virtual void Dispose()
        {
        }

        protected void ClearAllErrors()
        {
            ErrorMessage = string.Empty;
            HasCustomErrors = false;
            ClearErrors();
        }

        protected void SetError(string message)
        {
            ErrorMessage = message;
            HasCustomErrors = true;
        }

        [RelayCommand]
        protected virtual async Task RefreshCommand()
        {
            await RefreshAsync();
        }
    }
}
