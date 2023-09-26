using Microsoft.Extensions.Configuration;

namespace MonkeyTax.AWS.Lambda.FetchAndNotifyChanges.Configuration
{
    internal static class ConfigurationService
    {
        public static IConfiguration GetConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}
