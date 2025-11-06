using CRM.UI.ViewModels.Base;

namespace CRM.UI.Services.Navigation
{
    public interface INavigationService
    {
        Task NavigateToAsync<TViewModel>(object parameter = null) where TViewModel : ViewModelBase;
        Task NavigateToAsync(string viewModelName, object parameter = null);
        Task GoBackAsync();
        bool CanGoBack { get; }
        void ClearHistory();
    }
}
