namespace GamxorOila.Application.Common.Interfaces;

/// <summary>Vaqtni testlanadigan qilish uchun abstraksiya.</summary>
public interface IClock
{
    DateTimeOffset UtcNow { get; }
}
