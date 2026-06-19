using FluentValidation;
using GamxorOila.Application.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GamxorOila.Api.Filters;

/// <summary>
/// So'rov DTO'larini FluentValidation orqali tekshiradi. Xatolik bo'lsa HTTP 400
/// emas, mijoz kutadigan { success:false, message } shaklini qaytaradi.
/// </summary>
public class ContractValidationFilter(IServiceProvider serviceProvider) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is null) continue;

            var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());
            if (serviceProvider.GetService(validatorType) is not IValidator validator) continue;

            var validationContext = new ValidationContext<object>(argument);
            var result = await validator.ValidateAsync(validationContext, context.HttpContext.RequestAborted);
            if (!result.IsValid)
            {
                var message = result.Errors.FirstOrDefault()?.ErrorMessage ?? "Ma'lumotlar noto'g'ri.";
                context.Result = new OkObjectResult(ApiResponseDto.Fail(message));
                return;
            }
        }

        await next();
    }
}
