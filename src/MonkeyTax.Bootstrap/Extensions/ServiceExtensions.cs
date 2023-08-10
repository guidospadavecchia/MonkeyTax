using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MonkeyTax.Application.Monotributo.Services.Monotributo;
using MonkeyTax.Application.Monotributo.Services.Monotributo.Config;
using MonkeyTax.Application.UserAgents.Services;

namespace MonkeyTax.Bootstrap.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.Configure<MonotributoServiceConfig>(configuration.GetSection("Scraper"));
            serviceCollection.AddSingleton(services => services.GetRequiredService<IOptions<MonotributoServiceConfig>>().Value);
            serviceCollection.AddScoped<IMonotributoService, MonotributoService>();

            string? userAgentsUrl = configuration["Scraper:UserAgentsUrl"];
            serviceCollection.AddScoped<IUserAgentService, UserAgentService>(x => new UserAgentService(userAgentsUrl, x.GetRequiredService<IMemoryCache>()));

            return serviceCollection;
        }
    }
}
