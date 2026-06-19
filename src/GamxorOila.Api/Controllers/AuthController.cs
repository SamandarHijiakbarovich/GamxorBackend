using GamxorOila.Application.Contracts;
using GamxorOila.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace GamxorOila.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IFamilyCareService service) : ControllerBase
{
    [HttpPost("request-code")]
    [EnableRateLimiting("otp")]
    public async Task<ActionResult<ApiResponseDto>> RequestCode(
        [FromBody] RequestCodeRequest request, CancellationToken ct) =>
        Ok(await service.RequestCodeAsync(request, ct));

    [HttpPost("verify-code")]
    public async Task<ActionResult<ApiResponseDto>> VerifyCode(
        [FromBody] VerifyCodeRequest request, CancellationToken ct) =>
        Ok(await service.VerifyCodeAsync(request, ct));

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponseDto>> Register(
        [FromBody] RegisterRequest request, CancellationToken ct) =>
        Ok(await service.RegisterAsync(request, ct));
}
