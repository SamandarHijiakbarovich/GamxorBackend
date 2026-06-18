namespace GamxorOila.Application.Contracts;

/// <summary>bootstrap/ javobi: { "state": { ... } }.</summary>
public record BootstrapResponse
{
    public AppUiStateDto State { get; init; } = new();
}

/// <summary>Umumiy harakat javobi: { success, message, state? }.</summary>
public record ApiResponseDto
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public AppUiStateDto? State { get; init; }

    public static ApiResponseDto Ok(string message, AppUiStateDto? state = null) =>
        new() { Success = true, Message = message, State = state };

    public static ApiResponseDto Fail(string message, AppUiStateDto? state = null) =>
        new() { Success = false, Message = message, State = state };
}

/// <summary>uploads/assets/ javobi.</summary>
public record UploadResultDto
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public string? FileUrl { get; init; }
}
