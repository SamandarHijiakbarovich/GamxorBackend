using GamxorOila.Domain.Common;
using GamxorOila.Domain.Enums;

namespace GamxorOila.Domain.Entities;

/// <summary>Faol SOS sessiyasida xabardor qilingan kontakt.</summary>
public class SosContact : BaseEntity
{
    public Guid DeviceId { get; set; }
    public Device Device { get; set; } = null!;

    public int MemberAppId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public SosContactState State { get; set; } = SosContactState.Calling;
}
