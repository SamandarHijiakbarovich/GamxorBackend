namespace GamxorOila.Application.Contracts;

/// <summary>Barcha so'rovlar qurilma identifikatorini o'z ichiga oladi.</summary>
public record DeviceRequest
{
    public string DeviceId { get; init; } = string.Empty;
}

public record RequestCodeRequest : DeviceRequest
{
    public string Phone { get; init; } = string.Empty;
}

public record VerifyCodeRequest : DeviceRequest
{
    public string Phone { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
}

public record RegisterRequest : DeviceRequest
{
    public string FullName { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
}

public record SaveProfileRequest : DeviceRequest
{
    public CaregiverProfileDto Profile { get; init; } = new();
}

public record SelectMemberRequest : DeviceRequest
{
    public int MemberId { get; init; }
}

public record SendInvitationRequest : DeviceRequest
{
    public string Name { get; init; } = string.Empty;
    public string Relation { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
}
