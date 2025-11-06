using CRM.UI.ViewModels;
using CRM.UI.Views.Admin.EmployeeManagement;
using CRM.UI.Views.Admin.ProjectManagement;
using CRM.UI.Views.ContactManagement;
using CRM.UI.Views.ContractManagement;
using CRM.UI.Views.CustomerManagement;
using CRM.UI.Views.Dashboard;
using CRM.UI.Views.DepositManagement;
using CRM.UI.Views.LeadManagement;
using CRM.UI.Views.OpportunityManagement;
using CRM.UI.Views.PaymentManagement;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace CRM.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IServiceProvider _serviceProvider;

        private MainWindowViewModel _viewModel;
        private bool _isSidebarExpanded = true;
        private const double ExpandedWidth = 240;
        private const double CollapsedWidth = 60;

        // Store references to text elements for easy access
        private List<FrameworkElement> _sidebarTexts;
        private List<FrameworkElement> _sidebarHeaders;

        public MainWindow(MainWindowViewModel viewModel, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            InitializeSidebarElements();

            _viewModel = viewModel;
            DataContext = _viewModel;
            _serviceProvider = serviceProvider;
            //Content = new ShellView();
        }

        private void InitializeSidebarElements()
        {
            _sidebarTexts = new List<FrameworkElement>
            {
                DashboardText,
                AttendanceText,
                ScheduleText,
                EmployeeText,
                DepartmentText,
                DayOffText,
                PayrollText,
                ReportsText,
                BroadcastText,
                SettingsText,
                UserManagementText,
                HelpText,
                UserInfoPanel
            };

            _sidebarHeaders = new List<FrameworkElement>
            {
                MainSectionHeader,
                OthersSectionHeader
            };
        }

        private void MenuToggleButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleSidebar();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton)
            {
                // Update page title based on selected menu
                //UpdatePageTitle(radioButton.Name);

                // Load corresponding view/viewmodel
                LoadView(radioButton.Name);

                // Auto-collapse sidebar after selection
                if (_isSidebarExpanded)
                {
                    CollapseSidebar();
                }
            }
        }

        private void ToggleSidebar()
        {
            if (_isSidebarExpanded)
            {
                CollapseSidebar();
            }
            else
            {
                ExpandSidebar();
            }
        }

        private void CollapseSidebar()
        {
            // Create animation for sidebar width
            var animation = new DoubleAnimation
            {
                To = CollapsedWidth,
                Duration = TimeSpan.FromSeconds(0.3),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            // Create fade out animations for texts
            var fadeOutAnimation = new DoubleAnimation
            {
                To = 0,
                Duration = TimeSpan.FromSeconds(0.15)
            };

            // Apply animations
            Sidebar.BeginAnimation(Border.WidthProperty, animation);

            // Fade out texts and headers
            foreach (var text in _sidebarTexts)
            {
                text.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
            }

            foreach (var header in _sidebarHeaders)
            {
                header.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
            }

            // After animation completes, hide the texts
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.15);
            timer.Tick += (s, args) =>
            {
                foreach (var text in _sidebarTexts)
                {
                    text.Visibility = Visibility.Collapsed;
                }
                foreach (var header in _sidebarHeaders)
                {
                    header.Visibility = Visibility.Collapsed;
                }
                timer.Stop();
            };
            timer.Start();

            _isSidebarExpanded = false;
        }

        private void ExpandSidebar()
        {
            // Show texts first
            foreach (var text in _sidebarTexts)
            {
                text.Visibility = Visibility.Visible;
            }
            foreach (var header in _sidebarHeaders)
            {
                header.Visibility = Visibility.Visible;
            }

            // Create animation for sidebar width
            var animation = new DoubleAnimation
            {
                To = ExpandedWidth,
                Duration = TimeSpan.FromSeconds(0.3),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            // Create fade in animations for texts
            var fadeInAnimation = new DoubleAnimation
            {
                To = 1,
                Duration = TimeSpan.FromSeconds(0.2),
                BeginTime = TimeSpan.FromSeconds(0.1)
            };

            // Apply animations
            Sidebar.BeginAnimation(Border.WidthProperty, animation);

            // Fade in texts
            foreach (var text in _sidebarTexts)
            {
                text.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
            }

            foreach (var header in _sidebarHeaders)
            {
                header.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
            }

            _isSidebarExpanded = true;
        }

        private void UpdatePageTitle(string menuName)
        {
            var titles = new Dictionary<string, string>
            {
                { "DashboardMenu", "Dashboard" },
                { "LeadManagement", "Quản lý khách hàng tiềm năng" },
                { "ScheduleMenu", "Schedule" },
                { "EmployeeMenu", "Employee Directory" },
                { "DepartmentMenu", "Department" },
                { "DayOffMenu", "Day-Off Management" },
                { "PayrollMenu", "Payroll" },
                { "ReportsMenu", "Reports" },
                { "BroadcastMenu", "Broadcast" },
                { "SettingsMenu", "Settings" },
                { "UserManagementMenu", "User Management" },
                { "HelpMenu", "Help Center" }
            };

            if (titles.ContainsKey(menuName))
            {
                //   PageTitle.Text = titles[menuName];
            }
        }

        private void LoadView(string menuName)
        {
            // Get the ViewModel from DataContext
            if (DataContext is MainWindowViewModel)
            {
                // Update CurrentView based on selected menu
                switch (menuName)
                {
                    case "DashboardMenu":
                        _viewModel.CurrentView = _serviceProvider.GetRequiredService<DashboardView>();
                        break;
                    case "LeadManagement":
                        _viewModel.CurrentView = _serviceProvider.GetRequiredService<LeadManagementView>();
                        break;
                    case "CustomerManagement":
                        _viewModel.CurrentView = _serviceProvider.GetRequiredService<CustomerManagementView>();
                        break;
                    case "OpportunityManagement":
                        _viewModel.CurrentView = _serviceProvider.GetRequiredService<OpportunityManagement>();
                        break;
                    case "DepositManagement":
                        _viewModel.CurrentView = _serviceProvider.GetRequiredService<DepositManagementView>();
                        break;
                    case "ContractManagement":
                        _viewModel.CurrentView = _serviceProvider.GetRequiredService<ContractManagementView>();
                        break;
                    case "InvoiceManagement":
                        _viewModel.CurrentView = _serviceProvider.GetRequiredService<InvoiceManagementView>();
                        break;
                    case "PaymentManagement":
                        _viewModel.CurrentView = _serviceProvider.GetRequiredService<PaymentManagementView>();
                        break;
                    case "ContactManagement":
                        _viewModel.CurrentView = _serviceProvider.GetRequiredService<ContactManagementView>();
                        break;
                    case "EmployeeManagement":
                        _viewModel.CurrentView = _serviceProvider.GetRequiredService<EmployeeManagementView>();
                        break;
                    case "ProjectManagement":
                        _viewModel.CurrentView = _serviceProvider.GetRequiredService<ProjectManagementView>();
                        break;
                    default:
                        break;
                }
            }
        }

        private void Image_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is Image img && img.Source == null)
                MessageBox.Show("Logo not found!");
        }

    }
}
