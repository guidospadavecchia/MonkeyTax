using MonkeyTax.API.Enums;
using MonkeyTax.API.Routing.Base;
using MonkeyTax.Application.Monotributo.Services.Monotributo;

namespace MonkeyTax.API.Routing
{
    public class MonotributoRouter : ModuleRouter, IMonotributoRouter
    {
        private readonly IMonotributoService _monotributoService;

        public MonotributoRouter(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            _monotributoService = scope.ServiceProvider.GetRequiredService<IMonotributoService>();
        }

        public override void Map(WebApplication app)
        {
            app.MapGet("/api/monotributo/categorias", (CancellationToken cancellationToken) => _monotributoService.GetValuesAsync(cancellationToken))
               .WithTags(Modules.Monotributo.ToString())
               .WithSummary("Obtiene todos los montos y categorías vigentes")
               .WithOpenApi();
        }
    }
}
