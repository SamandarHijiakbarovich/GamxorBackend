using GamxorOila.Domain.Common;

namespace GamxorOila.Domain.Entities;

/// <summary>Faol SOS ogohlantirishi (qaysidir a'zo bo'yicha).</summary>
public class SosAlert : BaseEntity
{
    public Guid DeviceId { get; set; }
    public Device Device { get; set; } = null!;

    public int MemberAppId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Relation { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string AvatarUri { get; set; } = string.Empty;
    public DateTimeOffset RaisedAt { get; set; } = DateTimeOffset.UtcNow;
}
