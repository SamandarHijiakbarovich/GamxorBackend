using GamxorOila.Application.Contracts;
using GamxorOila.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace GamxorOila.Api.Controllers;

[ApiController]
public class FamilyController(IFamilyCareService service) : ControllerBase
{
    [HttpPost("api/profile/save")]
    public async Task<ActionResult<ApiResponseDto>> SaveProfile(
        [FromBody] SaveProfileRequest request, CancellationToken ct) =>
        Ok(await service.SaveProfileAsync(request, ct));

    [HttpPost("api/members/select")]
    public async Task<ActionResult<ApiResponseDto>> SelectMember(
        [FromBody] SelectMemberRequest request, CancellationToken ct) =>
        Ok(await service.SelectMemberAsync(request, ct));

    [HttpPost("api/invitations/send")]
    public async Task<ActionResult<ApiResponseDto>> SendInvitation(
        [FromBody] SendInvitationRequest request, CancellationToken ct) =>
        Ok(await service.SendInvitationAsync(request, ct));

    [HttpPost("api/invitations/{id:int}/accept")]
    public async Task<ActionResult<ApiResponseDto>> AcceptInvitation(
        int id, [FromBody] DeviceRequest request, CancellationToken ct) =>
        Ok(await service.AcceptInvitationAsync(request.DeviceId, id, ct));

    [HttpPost("api/notifications/{id:int}/dismiss")]
    public async Task<ActionResult<ApiResponseDto>> DismissNotification(
        int id, [FromBody] DeviceRequest request, CancellationToken ct) =>
        Ok(await service.DismissNotificationAsync(request.DeviceId, id, ct));

    [HttpPost("api/notifications/{id:int}/read")]
    public async Task<ActionResult<ApiResponseDto>> MarkNotificationRead(
        int id, [FromBody] DeviceRequest request, CancellationToken ct) =>
        Ok(await service.MarkNotificationReadAsync(request.DeviceId, id, ct));

    [HttpPost("api/notifications/read-all")]
    public async Task<ActionResult<ApiResponseDto>> MarkAllNotificationsRead(
        [FromBody] DeviceRequest request, CancellationToken ct) =>
        Ok(await service.MarkAllNotificationsReadAsync(request.DeviceId, ct));

    [HttpPost("api/sos/trigger")]
    public async Task<ActionResult<ApiResponseDto>> TriggerSos(
        [FromBody] DeviceRequest request, CancellationToken ct) =>
        Ok(await service.TriggerSosAsync(request.DeviceId, ct));

    [HttpPost("api/sos/clear")]
    public async Task<ActionResult<ApiResponseDto>> ClearSos(
        [FromBody] DeviceRequest request, CancellationToken ct) =>
        Ok(await service.ClearSosAsync(request.DeviceId, ct));
}
