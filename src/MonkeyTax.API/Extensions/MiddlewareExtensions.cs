using MonkeyTax.API.Routing.Middlewares;

namespace MonkeyTax.Bootstrap.Extensions
{
    public static class MiddlewareExtensions
    {
        public static void AddMiddlewares(this WebApplication app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
