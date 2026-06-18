namespace GamxorOila.Domain.Enums;

/// <summary>Holatlar Flutter mijozi kutadigan ENUM qiymatlariga moslangan.</summary>
public enum MemberStatus
{
    Safe,
    Moving,
    NeedsAttention
}

public enum FeedSeverity
{
    Positive,
    Neutral,
    Warning
}

public enum InvitationStatus
{
    PendingAcceptance,
    WaitingInstall,
    Accepted
}

public enum NotificationCategory
{
    Invite,
    Safety,
    Crime,
    System
}

public enum SosContactState
{
    Calling,
    Notified
}
