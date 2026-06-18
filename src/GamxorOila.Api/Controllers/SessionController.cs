using GamxorOila.Application.Contracts;
using GamxorOila.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace GamxorOila.Api.Controllers;

[ApiController]
[Route("api")]
public class SessionController(IFamilyCareService service) : ControllerBase
{
    /// <summary>Ilova ishga tushganda chaqiriladi — qurilma holatini qaytaradi.</summary>
    [HttpPost("bootstrap")]
    public async Task<ActionResult<BootstrapResponse>> Bootstrap(
        [FromBody] DeviceRequest request, CancellationToken ct) =>
        Ok(await service.BootstrapAsync(request.DeviceId, ct));

    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResponseDto>> Refresh(
        [FromBody] DeviceRequest request, CancellationToken ct) =>
        Ok(await service.RefreshAsync(request.DeviceId, ct));

    [HttpPost("sign-out")]
    public async Task<ActionResult<ApiResponseDto>> SignOut(
        [FromBody] DeviceRequest request, CancellationToken ct) =>
        Ok(await service.SignOutAsync(request.DeviceId, ct));
}
