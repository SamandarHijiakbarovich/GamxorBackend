namespace GamxorOila.Application.Contracts;

/// <summary>
/// Flutter mijozi (lib/data/models.dart) kutadigan to'liq UI holati.
/// JSON kalitlari camelCase, enum qiymatlari esa mijoz fromJson logikasiga mos
/// UPPER_CASE satrlar sifatida yuboriladi.
/// </summary>
public record AppUiStateDto
{
    public bool IsLoading { get; init; }
    public bool IsLoggedIn { get; init; }
    public bool IsBackendOnline { get; init; } = true;
    public bool IsRegistered { get; init; }
    public bool IsSendingCode { get; init; }
    public bool IsVerifying { get; init; }
    public bool IsRefreshing { get; init; }
    public bool OtpRequested { get; init; }
    public string OtpHint { get; init; } = "2580";
    public string? LoginError { get; init; }
    public string CaregiverName { get; init; } = "Siz";
    public string FamilyLabel { get; init; } = "Mening oilam";
    public string BackendStatusLabel { get; init; } = "Online";
    public string ServerBaseUrl { get; init; } = string.Empty;
    public string LastSyncLabel { get; init; } = "Hozirgina";
    public CaregiverProfileDto Profile { get; init; } = new();
    public FamilyMemberDto SelfMember { get; init; } = new();
    public List<FamilyMemberDto> Members { get; init; } = [];
    public List<InvitationDto> Invitations { get; init; } = [];
    public List<NotificationDto> Notifications { get; init; } = [];
    public List<ActivityFeedItemDto> ActivityFeed { get; init; } = [];
    public int? SelectedMemberId { get; init; }
    public string NextCheckInLabel { get; init; } = "Har 30 daqiqada yangilanadi";
    public int TrustedPlacesCount { get; init; } = 1;
    public List<SosAlertDto> ActiveSosAlerts { get; init; } = [];
    public SosUiStateDto SosState { get; init; } = new();
}

public record ProfilePermissionsDto
{
    public bool LocationEnabled { get; init; } = true;
    public bool MicrophoneEnabled { get; init; }
    public bool NotificationsEnabled { get; init; } = true;
    public bool PreciseLocationEnabled { get; init; } = true;
    public bool BackgroundRefreshEnabled { get; init; } = true;
}

public record CaregiverProfileDto
{
    public string FullName { get; init; } = "Yangi foydalanuvchi";
    public string Phone { get; init; } = "+998 ";
    public string Email { get; init; } = string.Empty;
    public string FamilyLabel { get; init; } = "Mening oilam";
    public string Address { get; init; } = "Toshkent";
    public string EmergencyContact { get; init; } = "+998 ";
    public int AvatarSeed { get; init; }
    public string AvatarUri { get; init; } = string.Empty;
    public string Bio { get; init; } = string.Empty;
    public ProfilePermissionsDto Permissions { get; init; } = new();
    public string MemberCode { get; init; } = string.Empty;
}

public record FamilyMemberDto
{
    public int Id { get; init; }
    public string MemberCode { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Relation { get; init; } = string.Empty;
    public int Age { get; init; }
    public double Lat { get; init; } = 41.3111;
    public double Lon { get; init; } = 69.2797;
    public string Address { get; init; } = string.Empty;
    public string PlaceLabel { get; init; } = string.Empty;
    public int Battery { get; init; } = 100;
    public int Steps { get; init; }
    public int HeartRate { get; init; }
    public string LastUpdate { get; init; } = "Hozirgina";
    /// <summary>SAFE | MOVING | NEEDS_ATTENTION</summary>
    public string Status { get; init; } = "SAFE";
    public string Note { get; init; } = string.Empty;
    public string SafeZone { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string Schedule { get; init; } = string.Empty;
    public int AvatarSeed { get; init; }
    public string AvatarUri { get; init; } = string.Empty;
    public double DistanceKm { get; init; }
    public bool IsCurrentUser { get; init; }
}

public record ActivityFeedItemDto
{
    public int Id { get; init; }
    public int? MemberId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Subtitle { get; init; } = string.Empty;
    public string TimeLabel { get; init; } = string.Empty;
    /// <summary>POSITIVE | NEUTRAL | WARNING</summary>
    public string Severity { get; init; } = "NEUTRAL";
}

public record InvitationDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Relation { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string SentAtLabel { get; init; } = string.Empty;
    /// <summary>PENDING_ACCEPTANCE | WAITING_INSTALL | ACCEPTED</summary>
    public string Status { get; init; } = "PENDING_ACCEPTANCE";
    public bool IsPlatformUser { get; init; }
    public bool CanAccept { get; init; }
    public string FamilyName { get; init; } = string.Empty;
    public string InvitedByName { get; init; } = string.Empty;
}

public record NotificationDto
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public string TimeLabel { get; init; } = string.Empty;
    /// <summary>INVITE | SAFETY | CRIME | SYSTEM</summary>
    public string Category { get; init; } = "SYSTEM";
    public bool IsRead { get; init; }
    public int? InviteId { get; init; }
    public string? ActionLabel { get; init; }
}

public record SosContactDto
{
    public int MemberId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    /// <summary>CALLING | NOTIFIED</summary>
    public string State { get; init; } = "CALLING";
}

public record SosAlertDto
{
    public int MemberId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Relation { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public string LastUpdate { get; init; } = string.Empty;
    public string AvatarUri { get; init; } = string.Empty;
}

public record SosUiStateDto
{
    public bool IsActive { get; init; }
    public bool IsSending { get; init; }
    public string Summary { get; init; } = string.Empty;
    public string SentAtLabel { get; init; } = string.Empty;
    public List<SosContactDto> Contacts { get; init; } = [];
}
