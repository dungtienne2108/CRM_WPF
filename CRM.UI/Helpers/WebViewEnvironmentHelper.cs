using Microsoft.Web.WebView2.Core;
using System.IO;

namespace CRM.UI.Helpers
{
    public static class WebViewEnvironmentHelper
    {
        public static CoreWebView2Environment? SharedEnvironment { get; private set; }

        public static async Task<CoreWebView2Environment> GetEnvironmentAsync()
        {
            if (SharedEnvironment == null)
            {
                string userDataFolder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "FLC_CRM_APP",
                    "WebView2Profile"
                );

                SharedEnvironment = await CoreWebView2Environment.CreateAsync(userDataFolder: userDataFolder);
            }

            return SharedEnvironment;
        }
    }

}
