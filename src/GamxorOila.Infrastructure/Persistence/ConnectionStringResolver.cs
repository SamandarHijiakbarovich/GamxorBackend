using Microsoft.Extensions.Configuration;
using Npgsql;

namespace GamxorOila.Infrastructure.Persistence;

/// <summary>
/// PostgreSQL ulanish satrini turli manbalardan aniqlaydi, shu sababli backend
/// muayyan hostingga bog'lanib qolmaydi:
///   1. DATABASE_URL muhit o'zgaruvchisi (postgres://user:pass@host:port/db) — Render, Railway, Heroku, Fly.io.
///   2. ConnectionStrings:Postgres (appsettings yoki muhit).
/// </summary>
public static class ConnectionStringResolver
{
    public static string Resolve(IConfiguration config)
    {
        var databaseUrl = config["DATABASE_URL"]
            ?? Environment.GetEnvironmentVariable("DATABASE_URL");

        if (!string.IsNullOrWhiteSpace(databaseUrl))
            return FromUrl(databaseUrl);

        var conn = config.GetConnectionString("Postgres");
        if (!string.IsNullOrWhiteSpace(conn))
            return conn;

        throw new InvalidOperationException(
            "PostgreSQL ulanishi topilmadi. DATABASE_URL yoki ConnectionStrings:Postgres ni belgilang.");
    }

    private static string FromUrl(string databaseUrl)
    {
        // postgres://user:password@host:port/database?sslmode=require
        var uri = new Uri(databaseUrl);
        var userInfo = uri.UserInfo.Split(':', 2);

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.IsDefaultPort ? 5432 : uri.Port,
            Username = Uri.UnescapeDataString(userInfo[0]),
            Password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : string.Empty,
            Database = uri.AbsolutePath.TrimStart('/')
        };

        // Bulutli Postgres provayderlari odatda SSL talab qiladi.
        var sslMode = ReadQueryValue(uri.Query, "sslmode");
        builder.SslMode = sslMode?.ToLowerInvariant() switch
        {
            "disable" => SslMode.Disable,
            "allow" => SslMode.Allow,
            "prefer" => SslMode.Prefer,
            "verify-ca" => SslMode.VerifyCA,
            "verify-full" => SslMode.VerifyFull,
            _ => SslMode.Require
        };

        return builder.ConnectionString;
    }

    private static string? ReadQueryValue(string query, string key)
    {
        var trimmed = query.TrimStart('?');
        if (trimmed.Length == 0) return null;

        foreach (var pair in trimmed.Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            var kv = pair.Split('=', 2);
            if (kv.Length == 2 && string.Equals(kv[0], key, StringComparison.OrdinalIgnoreCase))
                return Uri.UnescapeDataString(kv[1]);
        }
        return null;
    }
}
