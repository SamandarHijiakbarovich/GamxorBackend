using System.Text.Json.Serialization;
using GamxorOila.Application;
using GamxorOila.Infrastructure;
using GamxorOila.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Hosting platformasi (Render, Railway, Heroku va h.k.) bergan PORT ni qo'llab-quvvatlaymiz.
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(port))
    builder.WebHost.UseUrls($"http://+:{port}");

builder.Services
    .AddControllers()
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

const string CorsPolicy = "AllowApp";
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicy, policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

// Ma'lumotlar bazasini ishga tushirishda migratsiya qilamiz (bulutga deploy uchun qulay).
await ApplyMigrationsAsync(app);

if (app.Environment.IsDevelopment() ||
    app.Configuration.GetValue("EnableSwagger", false))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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

app.MapControllers();

app.MapGet("/", () => Results.Ok(new { service = "GamxorOila / Family Care API", status = "online" }));
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

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
