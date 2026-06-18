namespace GamxorOila.Domain.ValueObjects;

/// <summary>Foydalanuvchi profilidagi ruxsatlar (EF Core owned type sifatida saqlanadi).</summary>
public class ProfilePermissions
{
    public bool LocationEnabled { get; set; } = true;
    public bool MicrophoneEnabled { get; set; }
    public bool NotificationsEnabled { get; set; } = true;
    public bool PreciseLocationEnabled { get; set; } = true;
    public bool BackgroundRefreshEnabled { get; set; } = true;
}
