using GamxorOila.Application.Common.Interfaces;
using Microsoft.Extensions.Options;

namespace GamxorOila.Infrastructure.Services;

public class FileStorageOptions
{
    public const string Section = "FileStorage";

    /// <summary>Fayllar saqlanadigan fizik papka.</summary>
    public string RootPath { get; set; } = Path.Combine("wwwroot", "media");

    /// <summary>Mijozga qaytariladigan URL prefiksi.</summary>
    public string PublicPrefix { get; set; } = "/media";
}

public class LocalFileStorage(IOptions<FileStorageOptions> options) : IFileStorage
{
    private readonly FileStorageOptions _options = options.Value;

    public async Task<string> SaveAsync(Stream content, string originalFileName, string category, CancellationToken ct = default)
    {
        var safeCategory = Sanitize(string.IsNullOrWhiteSpace(category) ? "general" : category);
        var ext = Path.GetExtension(originalFileName);
        if (string.IsNullOrWhiteSpace(ext) || ext.Length > 10) ext = ".bin";

        var fileName = $"{Guid.NewGuid():N}{ext}";
        var dir = Path.Combine(_options.RootPath, "uploads", safeCategory);
        Directory.CreateDirectory(dir);

        var fullPath = Path.Combine(dir, fileName);
        await using (var fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
        {
            await content.CopyToAsync(fs, ct);
        }

        return $"{_options.PublicPrefix.TrimEnd('/')}/uploads/{safeCategory}/{fileName}";
    }

    private static string Sanitize(string value)
    {
        var cleaned = new string(value.Where(c => char.IsLetterOrDigit(c) || c is '_' or '-').ToArray());
        return cleaned.Length == 0 ? "general" : cleaned.ToLowerInvariant();
    }
}
