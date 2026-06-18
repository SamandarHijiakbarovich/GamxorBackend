namespace GamxorOila.Domain.Common;

/// <summary>Barcha agregatlar uchun umumiy kalit va audit maydonlari.</summary>
public abstract class BaseEntity
{
    // Kalit qiymatini EF Core generatsiya qiladi (ValueGeneratedOnAdd).
    // Oldindan Guid.NewGuid() berilsa, kuzatilayotgan agregatga qo'shilgan yangi
    // bola entity noto'g'ri "Modified" deb belgilanadi.
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}
