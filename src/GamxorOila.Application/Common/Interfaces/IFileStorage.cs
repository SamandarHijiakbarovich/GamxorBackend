namespace GamxorOila.Application.Common.Interfaces;

/// <summary>Yuklangan media fayllarni saqlash.</summary>
public interface IFileStorage
{
    /// <summary>Faylni saqlaydi va ildizga nisbatan URL qaytaradi (masalan, /media/uploads/abc.jpg).</summary>
    Task<string> SaveAsync(Stream content, string originalFileName, string category, CancellationToken ct = default);
}
