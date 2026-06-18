using GamxorOila.Domain.Common;
using GamxorOila.Domain.ValueObjects;

namespace GamxorOila.Domain.Entities;

/// <summary>
/// Agregat ildizi: bitta ilova o'rnatmasi (qurilma) = bitta hisob.
/// Mijoz autentifikatsiyasi <see cref="DeviceKey"/> orqali amalga oshiriladi.
/// </summary>
public class Device : BaseEntity
{
    /// <summary>Mijoz tomonidan yuboriladigan barqaror qurilma identifikatori.</summary>
    public string DeviceKey { get; set; } = string.Empty;

    public bool IsRegistered { get; set; }
    public bool IsLoggedIn { get; set; }
    public bool IsPhoneVerified { get; set; }

    public DateTimeOffset? LastSyncAt { get; set; }

    public int? SelectedMemberId { get; set; }
    public int TrustedPlacesCount { get; set; } = 1;
    public string NextCheckInLabel { get; set; } = "Har 30 daqiqada yangilanadi";

    // SOS sessiya holati
    public bool SosIsActive { get; set; }
    public bool SosIsSending { get; set; }
    public string SosSummary { get; set; } = string.Empty;
    public DateTimeOffset? SosSentAt { get; set; }

    public CaregiverProfile Profile { get; set; } = new();

    public List<FamilyMember> Members { get; set; } = [];
    public List<Invitation> Invitations { get; set; } = [];
    public List<AppNotification> Notifications { get; set; } = [];
    public List<ActivityFeedItem> ActivityFeed { get; set; } = [];
    public List<SosAlert> ActiveSosAlerts { get; set; } = [];
    public List<SosContact> SosContacts { get; set; } = [];
    public List<OtpRequest> OtpRequests { get; set; } = [];

    /// <summary>Qurilma doirasidagi kollektsiya uchun keyingi butun son AppId ni hisoblaydi.</summary>
    public int NextAppId(IEnumerable<int> existing)
    {
        var ids = existing.ToList();
        return ids.Count == 0 ? 1 : ids.Max() + 1;
    }
}
