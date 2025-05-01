using Microsoft.AspNetCore.Http;
using MyHealthProfile.Extensions;
namespace MyHealthProfile.Middlewares;

public class ErrorHandeler
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandeler> _logger;
    public ErrorHandeler(RequestDelegate next, ILogger<ErrorHandeler> logger)
    {
        _next = next;
        _logger = logger;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "error happen this is middleware");
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            var res = new GenericResult<string>
            {
                Status = false,
                ErrorMessage = ex.Message,
                Data = "this from the middleware"
            };
            await context.Response.WriteAsJsonAsync(res);
        }

    }
}

