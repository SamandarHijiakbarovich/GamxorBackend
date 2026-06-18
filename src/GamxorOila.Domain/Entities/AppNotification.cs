using GamxorOila.Domain.Common;
using GamxorOila.Domain.Enums;

namespace GamxorOila.Domain.Entities;

/// <summary>Ilova bildirishnomasi.</summary>
public class AppNotification : BaseEntity
{
    public Guid DeviceId { get; set; }
    public Device Device { get; set; } = null!;

    public int AppId { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTimeOffset OccurredAt { get; set; } = DateTimeOffset.UtcNow;
    public NotificationCategory Category { get; set; } = NotificationCategory.System;
    public bool IsRead { get; set; }
    public bool IsDismissed { get; set; }

    /// <summary>Taklifga bog'liq bildirishnoma uchun taklif AppId si.</summary>
    public int? InviteAppId { get; set; }

    public string? ActionLabel { get; set; }
}
