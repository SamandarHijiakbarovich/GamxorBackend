using GamxorOila.Domain.Enums;

namespace GamxorOila.Application.Mapping;

/// <summary>Domen enumlarini mijoz kutadigan satr qiymatlariga aylantiradi.</summary>
public static class EnumLabels
{
    public static string ToWire(this MemberStatus status) => status switch
    {
        MemberStatus.Moving => "MOVING",
        MemberStatus.NeedsAttention => "NEEDS_ATTENTION",
        _ => "SAFE"
    };

    public static string ToWire(this FeedSeverity severity) => severity switch
    {
        FeedSeverity.Positive => "POSITIVE",
        FeedSeverity.Warning => "WARNING",
        _ => "NEUTRAL"
    };

    public static string ToWire(this InvitationStatus status) => status switch
    {
        InvitationStatus.WaitingInstall => "WAITING_INSTALL",
        InvitationStatus.Accepted => "ACCEPTED",
        _ => "PENDING_ACCEPTANCE"
    };

    public static string ToWire(this NotificationCategory category) => category switch
    {
        NotificationCategory.Invite => "INVITE",
        NotificationCategory.Safety => "SAFETY",
        NotificationCategory.Crime => "CRIME",
        _ => "SYSTEM"
    };

    public static string ToWire(this SosContactState state) =>
        state == SosContactState.Notified ? "NOTIFIED" : "CALLING";
}
