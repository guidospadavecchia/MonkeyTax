using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MonkeyTax.AWS.Lambda.FetchAndNotifyChanges.Services;
using MonkeyTax.AWS.Lambda.FetchAndNotifyChanges.Services.Configuration;

namespace MonkeyTax.AWS.DynamoDB.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            MonotributoServiceConfig monotributoServiceConfig = new()
            {
                BaseUrl = configuration["API:BaseUrl"]!,
                GetCategoriesUrl = configuration["API:GetCategories"]!,
                TableName = configuration["AWS:DynamoDB:TableName"]!,
            };
            serviceCollection.AddSingleton(monotributoServiceConfig);
            serviceCollection.AddScoped<MonotributoService>();

            return serviceCollection;
        }
    }
}