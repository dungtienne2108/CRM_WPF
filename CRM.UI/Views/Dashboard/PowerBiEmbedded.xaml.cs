using Microsoft.Identity.Client;
using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;
using Microsoft.Rest;
using System.Windows;

namespace CRM.UI.Views.Dashboard
{
    /// <summary>
    /// Interaction logic for PowerBiEmbedded.xaml
    /// </summary>
    public partial class PowerBiEmbedded : Window
    {
        private static readonly string ClientId = "secret";
        private static readonly string TenantId = "secret";
        private static readonly string ClientSecret = "secredt"; // Hoặc dùng Certificate
        private static readonly Guid WorkspaceId = new Guid("secret");
        private static readonly Guid ReportId = new Guid("secret");

        public PowerBiEmbedded()
        {
            InitializeComponent();

            //Loaded += Window_Loaded;

            Loaded += LoadAsync;

            //PowerBiWebView.Source = new Uri("https://app.powerbi.com/reportEmbed?reportId=9f94d50d-93d7-42df-b752-eafc99d1bc89&autoAuth=true&ctid=30bad4e6-50f9-4842-b912-27ec8932e8ab");
        }

        private async void LoadAsync(object sender, RoutedEventArgs e)
        {
            await PowerBiWebView.EnsureCoreWebView2Async(null);

            string htmlPath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Resources", "report.html");
            PowerBiWebView.Source = new Uri(htmlPath, UriKind.Absolute);
        }

        public async Task<string> GetAccessTokenAsync()
        {
            // Cấu hình xác thực Service Principal (Application)
            IConfidentialClientApplication app = ConfidentialClientApplicationBuilder.Create(ClientId)
                .WithClientSecret(ClientSecret) // Hoặc WithCertificate
                .WithAuthority($"https://login.microsoftonline.com/{TenantId}")
                .Build();

            // Phạm vi truy cập Power BI Service
            string[] scopes = new string[] { "https://analysis.windows.net/powerbi/api/.default" };

            // Lấy Access Token
            var authResult = await app.AcquireTokenForClient(scopes).ExecuteAsync();
            return authResult.AccessToken;
        }


        public async Task<string> GetEmbedTokenAsync(string accessToken)
        {
            var credentials = new TokenCredentials(accessToken);
            using (var client = new PowerBIClient(new Uri("https://api.powerbi.com/"), credentials))
            {
                // Yêu cầu tạo Embed Token
                var embedTokenResponse = await client.Reports.GenerateTokenInGroupWithHttpMessagesAsync(
                    WorkspaceId,
                    ReportId,
                    new GenerateTokenRequest(TokenAccessLevel.View));

                return embedTokenResponse.Body.Token;
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var accessToken = await GetAccessTokenAsync();
            var embedToken = await GetEmbedTokenAsync(accessToken);
            var reportEmbedUrl = $"https://app.powerbi.com/reportEmbed?reportId={ReportId}&groupId={WorkspaceId}&autoAuth=true&ctid={TenantId}";

            var htmlContent = @"
        <html>
        <head>
            <script src='https://cdnjs.cloudflare.com/ajax/libs/powerbi-client/2.19.0/powerbi.min.js'></script>
        </head>
        <body>
            <div id='reportContainer' style='width:100%;height:100%;'></div>
        </body>
        </html>
    ";

            await PowerBiWebView.EnsureCoreWebView2Async();

            PowerBiWebView.CoreWebView2.NavigationCompleted += async (s, ev) =>
            {
                if (!ev.IsSuccess)
                {
                    return;
                }

                var script = $@"
            (function() {{
                var models = window['powerbi-client'].models;
                var embedConfiguration = {{
                    type: 'report',
                    id: '{ReportId}',
                    embedUrl: '{reportEmbedUrl}',
                    accessToken: '{embedToken}',
                    tokenType: 1,
                    settings: {{
                        filterPaneEnabled: false,
                        navContentPaneEnabled: false
                    }}
                }};
                var reportContainer = document.getElementById('reportContainer');
                var report = powerbi.embed(reportContainer, embedConfiguration);
            }})();
        ";
                await PowerBiWebView.CoreWebView2.ExecuteScriptAsync(script);
            };

            PowerBiWebView.CoreWebView2.NavigateToString(htmlContent);
        }
    }
}
