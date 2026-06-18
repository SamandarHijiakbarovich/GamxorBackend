using GamxorOila.Domain.Common;
using GamxorOila.Domain.Enums;

namespace GamxorOila.Domain.Entities;

/// <summary>Oila a'zosi (yoki foydalanuvchining o'zi — <see cref="IsSelf"/>).</summary>
public class FamilyMember : BaseEntity
{
    public Guid DeviceId { get; set; }
    public Device Device { get; set; } = null!;

    /// <summary>Mijozga ko'rinadigan butun son ID (qurilma doirasida noyob). O'zi uchun 0.</summary>
    public int AppId { get; set; }

    public bool IsSelf { get; set; }

    public string MemberCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Relation { get; set; } = string.Empty;
    public int Age { get; set; }
    public double Lat { get; set; } = 41.3111;
    public double Lon { get; set; } = 69.2797;
    public string Address { get; set; } = string.Empty;
    public string PlaceLabel { get; set; } = string.Empty;
    public int Battery { get; set; } = 100;
    public int Steps { get; set; }
    public int HeartRate { get; set; }
    public MemberStatus Status { get; set; } = MemberStatus.Safe;
    public string Note { get; set; } = string.Empty;
    public string SafeZone { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Schedule { get; set; } = string.Empty;
    public int AvatarSeed { get; set; }
    public string AvatarUri { get; set; } = string.Empty;
    public double DistanceKm { get; set; }

    /// <summary>Oxirgi yangilanish vaqti — UI uchun yorliq shu vaqtdan hosil qilinadi.</summary>
    public DateTimeOffset LastUpdate { get; set; } = DateTimeOffset.UtcNow;
}
