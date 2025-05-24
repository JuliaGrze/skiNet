using API.Errors;
using System.Net;
using System.Text.Json;

namespace API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly IHostEnvironment _environment; //It says, which enviroment you are (Development, Production, Staging)
        private readonly RequestDelegate next;
        public ExceptionMiddleware(IHostEnvironment environment, RequestDelegate requestDelegate)
        {
            _environment = environment;
            next = requestDelegate;
        }

        //main method evrry middleware - it's has to named InvokeAsync
        public async Task InvokeAsync(HttpContext context) //HttpContext - full information about request/response
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex, _environment);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex, IHostEnvironment environment)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; // 500 Internal Server Error

            var response = environment.IsDevelopment()
                ? new ApiErrorResponse(context.Response.StatusCode, ex.Message, ex.StackTrace)
                : new ApiErrorResponse(context.Response.StatusCode, ex.Message, "Internal server error");

            //Gdy będziesz zamieniać obiekt na JSON — użyj camelCase dla nazw właściwości
            //Pierwsza litera mała
            //Kolejne wyrazy — wielkie litery
            var option = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            var json = JsonSerializer.Serialize(response, option);

            await context.Response.WriteAsync(json);
        }
    }
}
