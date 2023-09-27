using MonkeyTax.API.Routing.Model;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace MonkeyTax.API.Routing.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                HttpStatusCode statusCode = ex switch
                {
                    ValidationException => HttpStatusCode.BadRequest,
                    KeyNotFoundException => HttpStatusCode.NotFound,
                    _ => HttpStatusCode.InternalServerError,
                };

                ErrorResponse errorResponse = new()
                {
                    StatusCode = (int)statusCode,
                    ErrorCode = statusCode.ToString(),
                    ErrorMessage = ex?.Message ?? string.Empty,
                    Exception = ex,
                };

                var result = JsonConvert.SerializeObject(errorResponse);
                context.Response.StatusCode = errorResponse.StatusCode;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(result);
            }
        }
    }
}
