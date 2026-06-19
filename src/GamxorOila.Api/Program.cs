using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using GamxorOila.Api.Filters;
using GamxorOila.Api.Infrastructure;
using GamxorOila.Application;
using GamxorOila.Application.Contracts;
using GamxorOila.Infrastructure;
using GamxorOila.Infrastructure.Persistence;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// ── Strukturali logging (Serilog) ──────────────────────────────────────────────
builder.Host.UseSerilog((context, cfg) => cfg
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console());

// Hosting platformasi (Render, Railway, Heroku va h.k.) bergan PORT ni qo'llab-quvvatlaymiz.
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(port))
    builder.WebHost.UseUrls($"http://+:{port}");

builder.Services
    .AddControllers(options => options.Filters.Add<ContractValidationFilter>())
    .AddJsonOptions(o =>
    {
        // Mijoz camelCase kalitlarni kutadi; null qiymatlar ham yuboriladi.
        o.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// ── Global xato-handler (ProblemDetails) ────────────────────────────────────────
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// ── Health-check'lar ────────────────────────────────────────────────────────────
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>("database", tags: ["ready"]);

// ── Reverse-proxy orqasidagi haqiqiy IP (Render va h.k.) ────────────────────────
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// ── Rate limiting (OTP so'roviga) ───────────────────────────────────────────────
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("otp", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1)
            }));

    // Limit oshganda HTTP 429 emas, mijoz kutadigan { success:false, message } qaytaramiz.
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status200OK;
        await context.HttpContext.Response.WriteAsJsonAsync(
            ApiResponseDto.Fail("Juda ko'p urinish. Bir daqiqadan so'ng qayta urinib ko'ring."),
            token);
    };
});

// ── CORS (sozlanadigan: ko'rsatilmasa hammaga ochiq — mobil ilova uchun) ────────
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
const string CorsPolicy = "AppCors";
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicy, policy =>
    {
        if (allowedOrigins.Length > 0)
            policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod();
        else
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

// Ma'lumotlar bazasini ishga tushirishda migratsiya qilamiz (bulutga deploy uchun qulay).
await ApplyMigrationsAsync(app);

app.UseForwardedHeaders();
app.UseExceptionHandler();
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment() ||
    app.Configuration.GetValue("EnableSwagger", false))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Xavfsizlik header'lari.
app.Use(async (context, next) =>
{
    var headers = context.Response.Headers;
    headers["X-Content-Type-Options"] = "nosniff";
    headers["X-Frame-Options"] = "DENY";
    headers["Referrer-Policy"] = "no-referrer";
    await next();
});

// Trailing slash'larni olib tashlaymiz — Flutter mijozi `bootstrap/` kabi yo'llarni yuboradi.
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value;
    if (!string.IsNullOrEmpty(path) && path.Length > 1 && path.EndsWith('/'))
        context.Request.Path = path.TrimEnd('/');
    await next();
});

app.UseStaticFiles(); // wwwroot/media/... fayllarini xizmat qiladi
app.UseCors(CorsPolicy);
app.UseRateLimiter();

app.MapControllers();

app.MapGet("/", () => Results.Ok(new { service = "GamxorOila / Family Care API", status = "online" }));

// Liveness — dastur tirikmi (bazaga tegmaydi). Render shu manzilni tekshiradi.
app.MapHealthChecks("/health", new HealthCheckOptions { Predicate = _ => false });

// Readiness — baza ham tayyormi.
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

app.Run();
return;

static async Task ApplyMigrationsAsync(WebApplication app)
{
    // Test muhitida sxema testlar tomonidan yaratiladi.
    if (app.Environment.IsEnvironment("Testing")) return;

    using var scope = app.Services.CreateScope();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
        logger.LogInformation("Ma'lumotlar bazasi migratsiyalari qo'llandi.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Ma'lumotlar bazasiga ulanib/migratsiya qilib bo'lmadi.");
    }
}

public partial class Program;
