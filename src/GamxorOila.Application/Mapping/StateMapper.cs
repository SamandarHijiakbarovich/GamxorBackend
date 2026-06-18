using GamxorOila.Application.Common;
using GamxorOila.Application.Contracts;
using GamxorOila.Domain.Entities;
using GamxorOila.Domain.ValueObjects;

namespace GamxorOila.Application.Mapping;

/// <summary>Device agregatidan mijozning AppUiState DTO sini quradi.</summary>
public static class StateMapper
{
    public static AppUiStateDto ToState(Device device, DateTimeOffset now)
    {
        var self = device.Members.FirstOrDefault(m => m.IsSelf);
        var others = device.Members
            .Where(m => !m.IsSelf)
            .OrderBy(m => m.AppId)
            .Select(m => ToMember(m, now))
            .ToList();

        var caregiverFirstName = FirstName(device.Profile.FullName);

        return new AppUiStateDto
        {
            IsLoading = false,
            IsLoggedIn = device.IsLoggedIn,
            IsBackendOnline = true,
            IsRegistered = device.IsRegistered,
            OtpRequested = false,
            OtpHint = "2580",
            CaregiverName = string.IsNullOrWhiteSpace(caregiverFirstName) ? "Siz" : caregiverFirstName,
            FamilyLabel = device.Profile.FamilyLabel,
            BackendStatusLabel = "Online",
            LastSyncLabel = device.LastSyncAt is { } sync ? TimeLabels.Relative(sync, now) : "Hozirgina",
            Profile = ToProfile(device.Profile),
            SelfMember = self is not null ? ToMember(self, now) : new FamilyMemberDto { IsCurrentUser = true, MemberCode = "SELF" },
            Members = others,
            Invitations = device.Invitations
                .OrderByDescending(i => i.SentAt)
                .Select(i => ToInvitation(i, now))
                .ToList(),
            Notifications = device.Notifications
                .Where(n => !n.IsDismissed)
                .OrderByDescending(n => n.OccurredAt)
                .Select(n => ToNotification(n, now))
                .ToList(),
            ActivityFeed = device.ActivityFeed
                .OrderByDescending(a => a.OccurredAt)
                .Select(a => ToActivity(a, now))
                .ToList(),
            SelectedMemberId = device.SelectedMemberId,
            NextCheckInLabel = device.NextCheckInLabel,
            TrustedPlacesCount = device.TrustedPlacesCount,
            ActiveSosAlerts = device.ActiveSosAlerts
                .OrderByDescending(a => a.RaisedAt)
                .Select(a => ToSosAlert(a, now))
                .ToList(),
            SosState = ToSosState(device, now)
        };
    }

    public static CaregiverProfileDto ToProfile(CaregiverProfile p) => new()
    {
        FullName = p.FullName,
        Phone = p.Phone,
        Email = p.Email,
        FamilyLabel = p.FamilyLabel,
        Address = p.Address,
        EmergencyContact = p.EmergencyContact,
        AvatarSeed = p.AvatarSeed,
        AvatarUri = p.AvatarUri,
        Bio = p.Bio,
        MemberCode = p.MemberCode,
        Permissions = new ProfilePermissionsDto
        {
            LocationEnabled = p.Permissions.LocationEnabled,
            MicrophoneEnabled = p.Permissions.MicrophoneEnabled,
            NotificationsEnabled = p.Permissions.NotificationsEnabled,
            PreciseLocationEnabled = p.Permissions.PreciseLocationEnabled,
            BackgroundRefreshEnabled = p.Permissions.BackgroundRefreshEnabled
        }
    };

    private static FamilyMemberDto ToMember(FamilyMember m, DateTimeOffset now) => new()
    {
        Id = m.AppId,
        MemberCode = m.MemberCode,
        Name = m.Name,
        Relation = m.Relation,
        Age = m.Age,
        Lat = m.Lat,
        Lon = m.Lon,
        Address = m.Address,
        PlaceLabel = m.PlaceLabel,
        Battery = m.Battery,
        Steps = m.Steps,
        HeartRate = m.HeartRate,
        LastUpdate = TimeLabels.Relative(m.LastUpdate, now),
        Status = m.Status.ToWire(),
        Note = m.Note,
        SafeZone = m.SafeZone,
        Phone = m.Phone,
        Schedule = m.Schedule,
        AvatarSeed = m.AvatarSeed,
        AvatarUri = m.AvatarUri,
        DistanceKm = m.DistanceKm,
        IsCurrentUser = m.IsSelf
    };

    private static InvitationDto ToInvitation(Invitation i, DateTimeOffset now) => new()
    {
        Id = i.AppId,
        Name = i.Name,
        Relation = i.Relation,
        Phone = i.Phone,
        SentAtLabel = TimeLabels.Relative(i.SentAt, now),
        Status = i.Status.ToWire(),
        IsPlatformUser = i.IsPlatformUser,
        CanAccept = i.CanAccept,
        FamilyName = i.FamilyName,
        InvitedByName = i.InvitedByName
    };

    private static NotificationDto ToNotification(AppNotification n, DateTimeOffset now) => new()
    {
        Id = n.AppId,
        Title = n.Title,
        Message = n.Message,
        TimeLabel = TimeLabels.Relative(n.OccurredAt, now),
        Category = n.Category.ToWire(),
        IsRead = n.IsRead,
        InviteId = n.InviteAppId,
        ActionLabel = n.ActionLabel
    };

    private static ActivityFeedItemDto ToActivity(ActivityFeedItem a, DateTimeOffset now) => new()
    {
        Id = a.AppId,
        MemberId = a.MemberAppId,
        Title = a.Title,
        Subtitle = a.Subtitle,
        TimeLabel = TimeLabels.Relative(a.OccurredAt, now),
        Severity = a.Severity.ToWire()
    };

    private static SosAlertDto ToSosAlert(SosAlert a, DateTimeOffset now) => new()
    {
        MemberId = a.MemberAppId,
        Name = a.Name,
        Relation = a.Relation,
        Phone = a.Phone,
        Address = a.Address,
        LastUpdate = TimeLabels.Relative(a.RaisedAt, now),
        AvatarUri = a.AvatarUri
    };

    private static SosUiStateDto ToSosState(Device device, DateTimeOffset now) => new()
    {
        IsActive = device.SosIsActive,
        IsSending = device.SosIsSending,
        Summary = device.SosSummary,
        SentAtLabel = device.SosSentAt is { } sent ? TimeLabels.Relative(sent, now) : string.Empty,
        Contacts = device.SosContacts
            .OrderBy(c => c.MemberAppId)
            .Select(c => new SosContactDto
            {
                MemberId = c.MemberAppId,
                Name = c.Name,
                Phone = c.Phone,
                State = c.State.ToWire()
            })
            .ToList()
    };

    private static string FirstName(string fullName)
    {
        var trimmed = (fullName ?? string.Empty).Trim();
        if (trimmed.Length == 0) return string.Empty;
        var first = trimmed.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? string.Empty;
        return first;
    }
}
