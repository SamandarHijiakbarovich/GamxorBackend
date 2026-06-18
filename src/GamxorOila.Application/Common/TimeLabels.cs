namespace GamxorOila.Application.Common;

/// <summary>UI uchun o'zbekcha nisbiy vaqt yorliqlarini hosil qiladi.</summary>
public static class TimeLabels
{
    public static string Relative(DateTimeOffset moment, DateTimeOffset now)
    {
        var diff = now - moment;
        if (diff < TimeSpan.Zero) diff = TimeSpan.Zero;

        if (diff.TotalSeconds < 60) return "Hozirgina";
        if (diff.TotalMinutes < 60) return $"{(int)diff.TotalMinutes} daqiqa oldin";
        if (diff.TotalHours < 24) return $"{(int)diff.TotalHours} soat oldin";
        if (diff.TotalDays < 7) return $"{(int)diff.TotalDays} kun oldin";
        return moment.ToString("dd.MM.yyyy");
    }
}
