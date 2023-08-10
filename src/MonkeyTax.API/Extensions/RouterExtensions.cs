using MonkeyTax.API.Routing;
using MonkeyTax.API.Routing.Base;

namespace MonkeyTax.Bootstrap.Extensions
{
    public static class RouterExtensions
    {
        public static IServiceCollection AddRouters(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IMonotributoRouter, MonotributoRouter>();
            serviceCollection.AddSingleton<IOtherRouter, OtherRouter>();
            return serviceCollection;
        }

        public static void MapRoutes(this WebApplication app)
        {
            List<IModuleRouter> moduleRouters = new()
            {
                app.Services.GetRequiredService<IMonotributoRouter>(),
                app.Services.GetRequiredService<IOtherRouter>(),
            };

            moduleRouters.ForEach(x => x.Map(app));
        }
    }
}
