using GamxorOila.Application.Common.Interfaces;
using GamxorOila.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GamxorOila.Infrastructure.Persistence;

public class DeviceRepository(AppDbContext db) : IDeviceRepository
{
    public Task<Device?> GetByKeyAsync(string deviceKey, CancellationToken ct = default) =>
        db.Devices
            .Include(d => d.Members)
            .Include(d => d.Invitations)
            .Include(d => d.Notifications)
            .Include(d => d.ActivityFeed)
            .Include(d => d.ActiveSosAlerts)
            .Include(d => d.SosContacts)
            .Include(d => d.OtpRequests)
            .FirstOrDefaultAsync(d => d.DeviceKey == deviceKey, ct);

    public async Task AddAsync(Device device, CancellationToken ct = default) =>
        await db.Devices.AddAsync(device, ct);

    public Task SaveChangesAsync(CancellationToken ct = default) => db.SaveChangesAsync(ct);
}
