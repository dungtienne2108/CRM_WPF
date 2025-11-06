using CRM.UI.ViewModels.Auth;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace CRM.UI.Views.Auth
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly LoginViewModel _viewModel;
        public LoginWindow(LoginViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;

            Loaded += OnLoaded;

            //DataContextChanged += OnDataContextChanged;

        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Set focus to username field
            UsernameTextBox.Focus();
            UsernameTextBox.SelectAll();

            // Apply fade-in animation
            ApplyFadeInAnimation();
        }

        //private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        //{
        //    _viewModel = DataContext as LoginViewModel;

        //    if (_viewModel != null)
        //    {
        //        // Sync password with ViewModel on load if Remember Me was checked
        //        if (!string.IsNullOrEmpty(_viewModel.Password))
        //        {
        //            PasswordBox.Password = _viewModel.Password;
        //        }
        //    }
        //}

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // Allow window dragging
                DragMove();
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel?.IsLoading == true)
            {
                var result = MessageBox.Show(
                    "Bạn có muốn thoát?",
                    "Xác nhận",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes)
                    return;
            }

            // Close application
            System.Windows.Application.Current.Shutdown();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
            {
                var passwordBox = sender as PasswordBox;

                //if (passwordBox?.Password == "#$1af%adk*" && _viewModel.RememberMe)
                //{
                //    return;
                //}

                _viewModel.Password = passwordBox?.Password ?? string.Empty;
            }
        }

        private void ApplyFadeInAnimation()
        {
            // Create fade-in animation for the window
            var fadeIn = new DoubleAnimation
            {
                From = 0.0,
                To = 1.0,
                Duration = new Duration(TimeSpan.FromMilliseconds(500)),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
            };

            BeginAnimation(OpacityProperty, fadeIn);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            // Handle ESC key to close window
            if (e.Key == Key.Escape)
            {
                CloseButton_Click(this, null);
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            // Remove icon from title bar (for cleaner look)
            //IconHelper.RemoveIcon(this);
        }
    }
}
