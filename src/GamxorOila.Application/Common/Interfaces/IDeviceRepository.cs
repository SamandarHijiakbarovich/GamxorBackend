using GamxorOila.Domain.Entities;

namespace GamxorOila.Application.Common.Interfaces;

/// <summary>Device agregati uchun saqlash abstraksiyasi (barcha bolalar bilan birga yuklanadi).</summary>
public interface IDeviceRepository
{
    Task<Device?> GetByKeyAsync(string deviceKey, CancellationToken ct = default);
    Task AddAsync(Device device, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
