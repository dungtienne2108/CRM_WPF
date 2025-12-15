using CRM.Application.Interfaces;
using CRM.Application.Interfaces.Auth;
using CRM.Application.Interfaces.Contact;
using CRM.Application.Interfaces.Contract;
using CRM.Application.Interfaces.Customers;
using CRM.Application.Interfaces.Deposit;
using CRM.Application.Interfaces.Email;
using CRM.Application.Interfaces.Employee;
using CRM.Application.Interfaces.Leads;
using CRM.Application.Interfaces.Opportunity;
using CRM.Application.Interfaces.Payment;
using CRM.Application.Interfaces.Project;
using CRM.Application.Services;
using CRM.Infrastructure;
using CRM.Infrastructure.Services;
using CRM.UI.Services.Messages;
using CRM.UI.Services.Navigation;
using CRM.UI.ViewModels;
using CRM.UI.ViewModels.Admin.EmployeeManagement;
using CRM.UI.ViewModels.Admin.ProjectManagement;
using CRM.UI.ViewModels.Auth;
using CRM.UI.ViewModels.ContactManagement;
using CRM.UI.ViewModels.ContractManagement;
using CRM.UI.ViewModels.CustomerManagement;
using CRM.UI.ViewModels.Dashboard;
using CRM.UI.ViewModels.DepositManagement;
using CRM.UI.ViewModels.LeadManagement;
using CRM.UI.ViewModels.OpportunityManagement;
using CRM.UI.ViewModels.PaymentManagement;
using CRM.UI.Views.Admin.EmployeeManagement;
using CRM.UI.Views.Admin.ProjectManagement;
using CRM.UI.Views.Auth;
using CRM.UI.Views.ContactManagement;
using CRM.UI.Views.ContractManagement;
using CRM.UI.Views.CustomerManagement;
using CRM.UI.Views.Dashboard;
using CRM.UI.Views.DepositManagement;
using CRM.UI.Views.LeadManagement;
using CRM.UI.Views.OpportunityManagement;
using CRM.UI.Views.PaymentManagement;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace CRM.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private IHost? _host;
        private IServiceProvider? _serviceProvider;

        public App()
        {
            ConfigureLogging();
            this.DispatcherUnhandledException += OnUnhandledException;
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _host = CreateHostBuilder(e.Args).Build();
            _serviceProvider = _host.Services;

            try
            {
                Log.Information("Application starting up");

                // Start the host
                await _host.StartAsync();

                // Create and show main window
                var mainWindow = _serviceProvider.GetRequiredService<LoginWindow>();
                mainWindow.Show();

                // Initialize navigation
                InitializeNavigation();

                // Load initial view
                LoadInitialView();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
                MessageBox.Show($"Application failed to start: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            try
            {
                Log.Information("Application shutting down");
                await _host?.StopAsync();
                _host?.Dispose();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error during application shutdown");
            }
            finally
            {
                Log.CloseAndFlush();
                base.OnExit(e);
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                          .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                          .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json",
                              optional: true, reloadOnChange: true)
                          .AddEnvironmentVariables()
                          .AddCommandLine(args);
                })
                .ConfigureServices((context, services) =>
                {
                    ConfigureServices(context.Configuration, services);
                })
                .ConfigureLogging((context, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddSerilog();
                });
        }

        private static void ConfigureServices(IConfiguration configuration, IServiceCollection services)
        {
            services.AddMemoryCache();

            services.AddInfrastructure(configuration);

            ConfigureApplicationServices(services);

            ConfigureUIServices(services);

            RegisterViewModels(services);

            RegisterViews(services);

            services.AddAutoMapper(config =>
            {
                config.AddMaps(Application.AssemblyReference.Assembly);
            });

            //services.AddHttpClient();
        }

        private static void ConfigureApplicationServices(IServiceCollection services)
        {
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IPasswordService, PasswordService>();

            //email
            services.AddScoped<IEmailService, EmailService>();

            // lead management
            services.AddScoped<ILeadService, LeadService>();

            // customer
            services.AddScoped<ICustomerService, CustomerService>();

            // opportunity
            services.AddScoped<IOpportunityService, OpportunityService>();

            // project
            services.AddScoped<IProjectService, ProjectService>();

            // employee
            services.AddScoped<IEmployeeService, EmployeeService>();

            // contact
            services.AddScoped<IContactService, ContactService>();

            //deposti
            services.AddScoped<IDepositService, DepositService>();

            // contract
            services.AddScoped<IContractService, ContractService>();

            // payment
            services.AddScoped<IPaymentService, PaymentService>();
        }

        private static void ConfigureUIServices(IServiceCollection services)
        {
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<CRM.UI.Services.Dialog.IDialogService, Services.Dialog.DialogService>();
            services.AddSingleton<IMessageService, MessageService>();
            services.AddSingleton<ISessionService, SessionService>();
            services.AddScoped<IUploadService, UploadService>();

        }

        private static void RegisterViewModels(IServiceCollection services)
        {
            services.AddTransient<MainWindowViewModel>();
            services.AddTransient<UserInfoViewModel>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<ForgotPasswordViewModel>();
            services.AddTransient<Func<string, ResetPasswordViewModel>>(sp => email =>
            {
                return new ResetPasswordViewModel(email, sp.GetRequiredService<IAuthenticationService>(), sp.GetRequiredService<IServiceProvider>());
            });
            services.AddSingleton<DashboardView>();

            // lead mângement
            //services.AddTransient<LeadItemViewModel>();
            services.AddTransient<LeadManagementViewModel>();
            services.AddTransient<AddLeadViewModel>();
            services.AddTransient<ConvertStageViewModel>();
            services.AddTransient<AddProductToLeadViewModel>();

            // customer management
            //services.AddTransient<CustomerItemViewModel>();
            services.AddTransient<CustomerManagementViewModel>();
            services.AddTransient<CustomerDetailViewModel>();
            services.AddTransient<AddCustomerViewModel>();

            // opportunity management
            services.AddTransient<OpportunityManagementViewModel>();
            services.AddTransient<AddOpportunityViewModel>();
            services.AddTransient<OpportunityDetailViewModel>();
            services.AddTransient<AddProductToOpportunityViewModel>();
            //services.AddTransient<OpportunityItemViewModel>();

            //deposit
            services.AddTransient<DepositManagementViewModel>();
            services.AddTransient<AddDepositViewModel>();
            services.AddTransient<DepositDetailViewModel>();

            // invoice & payment
            services.AddTransient<PaymentManagementViewModel>();
            services.AddTransient<AddPaymentViewModel>();
            services.AddTransient<InvoiceManagementViewModel>();
            services.AddTransient<AddPaymentScheduleViewModel>();
            services.AddTransient<AddInvoiceViewModel>();
            services.AddTransient<PaymentDetailViewModel>();
            services.AddTransient<InvoiceDetailViewModel>();

            // contract
            services.AddTransient<ContractManagementViewModel>();
            services.AddTransient<ContractDetailViewModel>();
            services.AddTransient<AddContractViewModel>();

            // contact
            services.AddTransient<ContactManagementViewModel>();
            services.AddTransient<AddContactViewModel>();
            services.AddTransient<ContactDetailViewModel>();

            //admin
            services.AddTransient<EmployeeManagementViewModel>();
            services.AddTransient<AddEmployeeViewModel>();
            services.AddTransient<EmployeeDetailViewModel>();
            services.AddTransient<ProjectManagementViewModel>();
            services.AddTransient<ProjectDetailViewModel>();
            services.AddTransient<ProductDetailViewModel>();
            services.AddTransient<AddProjectViewModel>();
        }

        private static void RegisterViews(IServiceCollection services)
        {
            services.AddTransient<MainWindow>();
            services.AddTransient<UserInfoView>();
            services.AddTransient<LoginWindow>();
            services.AddTransient<ForgotPasswordWindow>();

            // lead management
            services.AddTransient<LeadManagementView>();
            services.AddTransient<AddLeadDialog>();
            services.AddTransient<ConvertDialog>();

            // customer management
            services.AddTransient<CustomerManagementView>();

            // opportunity management
            services.AddTransient<OpportunityManagement>();

            // deposit
            services.AddTransient<DepositManagementView>();
            services.AddTransient<AddDepositDialog>();

            // payment
            services.AddTransient<PaymentManagementView>();
            services.AddTransient<InvoiceManagementView>();

            // contract
            services.AddTransient<ContractManagementView>();
            services.AddTransient<ContractDetailView>();

            // contact
            services.AddTransient<ContactManagementView>();
            services.AddTransient<AddContactDialog>();

            // admin
            services.AddTransient<EmployeeManagementView>();
            services.AddTransient<ProjectManagementView>();
            services.AddTransient<ProjectDetailView>();
        }

        private static void ConfigureLogging()
        {
            try
            {
                // Sử dụng Environment.SpecialFolder để đảm bảo path hợp lệ
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string logsPath = Path.Combine(baseDirectory, "logs");

                // Tạo folder nếu nó không tồn tại
                if (!Directory.Exists(logsPath))
                {
                    Directory.CreateDirectory(logsPath);
                    System.Diagnostics.Debug.WriteLine($"[DEBUG] Logs folder created at: {logsPath}");
                }

                string logFilePath = Path.Combine(logsPath, "crm-.log");

                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
                    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Warning)
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("Application", "CRM")
                    .WriteTo.File(
                        path: logFilePath,
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 30,
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                        shared: true) // Cho phép multiple processes ghi log
                    .CreateLogger();

                System.Diagnostics.Debug.WriteLine($"[DEBUG] Log file path: {logFilePath}");
                Log.Information("Logging configured successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR] Failed to configure logging: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[ERROR] Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        private void InitializeNavigation()
        {
            var navigationService = _serviceProvider.GetRequiredService<INavigationService>();
        }

        private void LoadInitialView()
        {
            var navigationService = _serviceProvider.GetRequiredService<INavigationService>();
            //await navigationService.NavigateToAsync<DashboardViewModel>();
        }

        private void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Log.Error(e.Exception, "Lỗi không xác định");

            MessageBox.Show(
                $"Lỗi không xác định: {e.Exception.Message}",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            e.Handled = true;
        }
    }
}
