using GamxorOila.Domain.Common;
using GamxorOila.Domain.Enums;

namespace GamxorOila.Domain.Entities;

/// <summary>Oilaga taklif.</summary>
public class Invitation : BaseEntity
{
    public Guid DeviceId { get; set; }
    public Device Device { get; set; } = null!;

    public int AppId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Relation { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTimeOffset SentAt { get; set; } = DateTimeOffset.UtcNow;
    public InvitationStatus Status { get; set; } = InvitationStatus.PendingAcceptance;
    public bool IsPlatformUser { get; set; }
    public bool CanAccept { get; set; }
    public string FamilyName { get; set; } = string.Empty;
    public string InvitedByName { get; set; } = string.Empty;
}
