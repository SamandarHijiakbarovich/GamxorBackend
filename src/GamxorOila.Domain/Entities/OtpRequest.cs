using GamxorOila.Domain.Common;

namespace GamxorOila.Domain.Entities;

/// <summary>SMS tasdiqlash kodi so'rovi.</summary>
public class OtpRequest : BaseEntity
{
    public Guid DeviceId { get; set; }
    public Device Device { get; set; } = null!;

    /// <summary>Kanonik (12 raqamli) telefon.</summary>
    public string Phone { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; set; }
    public bool Consumed { get; set; }

    public bool IsValid(string code, DateTimeOffset now) =>
        !Consumed && ExpiresAt > now && string.Equals(Code, code, StringComparison.Ordinal);
}
