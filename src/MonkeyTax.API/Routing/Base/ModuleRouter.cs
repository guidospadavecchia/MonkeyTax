namespace MonkeyTax.API.Routing.Base
{
    public abstract class ModuleRouter : IModuleRouter
    {
        public abstract void Map(WebApplication app);
    }
}
