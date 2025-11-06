using CommunityToolkit.Mvvm.Input;

namespace CRM.UI.ViewModels.Base
{
    public abstract partial class DialogViewModelBase<TResult> : ViewModelBase
    {
        private bool? _dialogResult;
        private TResult _result;

        public bool? DialogResult
        {
            get => _dialogResult;
            set => SetProperty(ref _dialogResult, value);
        }

        public TResult Result
        {
            get => _result;
            protected set => SetProperty(ref _result, value);
        }

        [RelayCommand]
        protected virtual void OK()
        {
            if (Validate())
            {
                PrepareResult();
                DialogResult = true;
            }
        }

        [RelayCommand]
        protected virtual void Cancel()
        {
            DialogResult = false;
        }

        protected abstract bool Validate();
        protected abstract void PrepareResult();
    }
}
