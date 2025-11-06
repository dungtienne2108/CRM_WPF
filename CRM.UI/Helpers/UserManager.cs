using System.IO;
using System.Text.Json;

namespace CRM.UI.Helpers
{
    public class UserConfig
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public bool RememberMe { get; set; } = false;
    }

    public static class ConfigManager
    {
        private static readonly string FilePath = "userConfig.json";

        public static void Save(UserConfig config)
        {
            var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, json);
        }

        public static UserConfig Load()
        {
            if (!File.Exists(FilePath))
                return new UserConfig();

            var json = File.ReadAllText(FilePath);
            if (string.IsNullOrWhiteSpace(json))
                return new UserConfig();

            try
            {
                return JsonSerializer.Deserialize<UserConfig>(json) ?? new UserConfig();
            }
            catch
            {
                return new UserConfig();
            }
        }
    }
}
