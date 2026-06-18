using GamxorOila.Application.Common.Interfaces;

namespace GamxorOila.Infrastructure.Services;

public class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
