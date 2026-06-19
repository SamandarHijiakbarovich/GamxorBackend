using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace GamxorOila.Api.Infrastructure;

/// <summary>Kutilmagan xatoliklarni ushlab, toza ProblemDetails JSON qaytaradi.</summary>
public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception,
            "Kutilmagan xatolik {Method} {Path}", httpContext.Request.Method, httpContext.Request.Path);

        var problem = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Serverda kutilmagan xatolik yuz berdi.",
            Detail = "Iltimos, birozdan so'ng qayta urinib ko'ring.",
            Type = "https://httpstatuses.com/500"
        };

        httpContext.Response.StatusCode = problem.Status.Value;
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }
}
