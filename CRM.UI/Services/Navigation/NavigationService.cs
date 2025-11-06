using CRM.UI.ViewModels.Base;
using System.Windows.Controls;

namespace CRM.UI.Services.Navigation
{
    public sealed class NavigationService : INavigationService
    {
        private readonly Dictionary<string, Type> _viewModelTypes = new();
        private readonly Dictionary<Type, Type> _viewModelToViewMapping = new();
        private readonly Stack<ViewModelBase> _navigationStack = new();
        private Frame _navigationFrame;

        public bool CanGoBack => _navigationStack.Count > 1;

        public void Initialize(Frame navigationFrame)
        {
            _navigationFrame = navigationFrame;
        }

        public void RegisterViewModel<TViewModel, TView>()
            where TViewModel : ViewModelBase
            where TView : Page
        {
            var viewModelType = typeof(TViewModel);
            var viewType = typeof(TView);

            _viewModelTypes[viewModelType.Name] = viewModelType;
            _viewModelToViewMapping[viewModelType] = viewType;
        }

        public async Task NavigateToAsync<TViewModel>(object parameter = null)
            where TViewModel : ViewModelBase
        {
            await NavigateToAsync(typeof(TViewModel), parameter);
        }

        public async Task NavigateToAsync(string viewModelName, object parameter = null)
        {
            if (_viewModelTypes.TryGetValue(viewModelName, out var viewModelType))
            {
                await NavigateToAsync(viewModelType, parameter);
            }
            else
            {
                throw new ArgumentException($"ViewModel {viewModelName} not registered");
            }
        }

        public async Task GoBackAsync()
        {
            if (CanGoBack)
            {
                _navigationStack.Pop(); // Remove current
                var previousViewModel = _navigationStack.Peek();

                if (_viewModelToViewMapping.TryGetValue(previousViewModel.GetType(), out var viewType))
                {
                    var view = Activator.CreateInstance(viewType) as Page;
                    view.DataContext = previousViewModel;
                    _navigationFrame.Navigate(view);

                    await previousViewModel.InitializeAsync();
                }
            }
        }

        public void ClearHistory()
        {
            _navigationStack.Clear();
        }

        private async Task NavigateToAsync(Type viewModelType, object parameter)
        {
            if (_viewModelToViewMapping.TryGetValue(viewModelType, out var viewType))
            {
                var viewModel = Activator.CreateInstance(viewModelType) as ViewModelBase;
                var view = Activator.CreateInstance(viewType) as Page;

                view.DataContext = viewModel;

                _navigationStack.Push(viewModel);
                _navigationFrame.Navigate(view);

                await viewModel.InitializeAsync(parameter);
            }
        }
    }
}
