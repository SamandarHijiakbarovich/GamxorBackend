using GamxorOila.Domain.Common;
using GamxorOila.Domain.Enums;

namespace GamxorOila.Domain.Entities;

/// <summary>Bosh ekrandagi faollik tasmasi elementi.</summary>
public class ActivityFeedItem : BaseEntity
{
    public Guid DeviceId { get; set; }
    public Device Device { get; set; } = null!;

    public int AppId { get; set; }

    /// <summary>Tegishli a'zoning AppId si (ixtiyoriy).</summary>
    public int? MemberAppId { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public DateTimeOffset OccurredAt { get; set; } = DateTimeOffset.UtcNow;
    public FeedSeverity Severity { get; set; } = FeedSeverity.Neutral;
}
