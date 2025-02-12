using Serilog;
using System.Net;
using System.Text.Json;

namespace ProjectBankAPI.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); // Continuar con la siguiente capa del middleware
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            Log.Error(ex, "Error no controlado en la API: {Message}", ex.Message);

            var response = new
            {
                message = ex.Message // Solo mostramos el mensaje sin stack trace
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest; // Código 400 para errores de negocio
            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
