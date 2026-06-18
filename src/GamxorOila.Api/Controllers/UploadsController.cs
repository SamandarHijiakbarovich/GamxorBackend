using GamxorOila.Application.Contracts;
using GamxorOila.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace GamxorOila.Api.Controllers;

[ApiController]
[Route("api/uploads")]
public class UploadsController(IFamilyCareService service) : ControllerBase
{
    /// <summary>Multipart fayl yuklash (masalan, profil avatari).</summary>
    [HttpPost("assets")]
    [RequestSizeLimit(15 * 1024 * 1024)]
    public async Task<ActionResult<UploadResultDto>> UploadAsset(
        [FromForm] string deviceId,
        [FromForm] string? category,
        [FromForm] string? title,
        IFormFile? file,
        CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            return Ok(new UploadResultDto { Success = false, Message = "Fayl topilmadi." });

        await using var stream = file.OpenReadStream();
        var result = await service.UploadAssetAsync(
            deviceId, stream, file.FileName,
            category ?? "general", title ?? string.Empty, ct);
        return Ok(result);
    }
}
