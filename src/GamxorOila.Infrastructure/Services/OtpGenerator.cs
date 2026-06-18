using System.Security.Cryptography;
using GamxorOila.Application.Common.Interfaces;
using Microsoft.Extensions.Options;

namespace GamxorOila.Infrastructure.Services;

public class OtpOptions
{
    public const string Section = "Otp";

    /// <summary>Bo'sh bo'lsa tasodifiy 4 xonali kod yaratiladi. Demo uchun "2580" ishlatiladi.</summary>
    public string? FixedCode { get; set; } = "2580";
}

public class OtpGenerator(IOptions<OtpOptions> options) : IOtpGenerator
{
    private readonly OtpOptions _options = options.Value;

    public string Generate()
    {
        if (!string.IsNullOrWhiteSpace(_options.FixedCode))
            return _options.FixedCode.Trim();

        return RandomNumberGenerator.GetInt32(1000, 10000).ToString();
    }
}
