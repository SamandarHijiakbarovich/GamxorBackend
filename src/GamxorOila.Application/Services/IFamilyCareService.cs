using GamxorOila.Application.Contracts;

namespace GamxorOila.Application.Services;

/// <summary>Family Care backendining barcha use-case'larini ifodalovchi servis.</summary>
public interface IFamilyCareService
{
    Task<BootstrapResponse> BootstrapAsync(string deviceId, CancellationToken ct = default);
    Task<ApiResponseDto> RequestCodeAsync(RequestCodeRequest request, CancellationToken ct = default);
    Task<ApiResponseDto> VerifyCodeAsync(VerifyCodeRequest request, CancellationToken ct = default);
    Task<ApiResponseDto> RegisterAsync(RegisterRequest request, CancellationToken ct = default);
    Task<ApiResponseDto> SaveProfileAsync(SaveProfileRequest request, CancellationToken ct = default);
    Task<ApiResponseDto> SelectMemberAsync(SelectMemberRequest request, CancellationToken ct = default);
    Task<ApiResponseDto> RefreshAsync(string deviceId, CancellationToken ct = default);
    Task<ApiResponseDto> SendInvitationAsync(SendInvitationRequest request, CancellationToken ct = default);
    Task<ApiResponseDto> AcceptInvitationAsync(string deviceId, int inviteId, CancellationToken ct = default);
    Task<ApiResponseDto> DismissNotificationAsync(string deviceId, int notificationId, CancellationToken ct = default);
    Task<ApiResponseDto> MarkNotificationReadAsync(string deviceId, int notificationId, CancellationToken ct = default);
    Task<ApiResponseDto> MarkAllNotificationsReadAsync(string deviceId, CancellationToken ct = default);
    Task<ApiResponseDto> TriggerSosAsync(string deviceId, CancellationToken ct = default);
    Task<ApiResponseDto> ClearSosAsync(string deviceId, CancellationToken ct = default);
    Task<ApiResponseDto> SignOutAsync(string deviceId, CancellationToken ct = default);

    Task<UploadResultDto> UploadAssetAsync(
        string deviceId, Stream content, string fileName, string category, string title,
        CancellationToken ct = default);
}
