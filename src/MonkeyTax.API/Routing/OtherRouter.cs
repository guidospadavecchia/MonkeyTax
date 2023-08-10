using MonkeyTax.API.Enums;
using MonkeyTax.API.Routing.Base;

namespace MonkeyTax.API.Routing
{
    public class OtherRouter : ModuleRouter, IOtherRouter
    {
        public override void Map(WebApplication app)
        {
            app.MapGet("/api/live", () => "OK")
               .WithTags(Modules.Otros.ToString())
               .WithSummary("Endpoint para corroborar el estado de la aplicación")
               .WithOpenApi();
        }
    }
}
