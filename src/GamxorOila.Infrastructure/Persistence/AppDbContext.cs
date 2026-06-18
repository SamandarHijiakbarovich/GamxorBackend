using GamxorOila.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GamxorOila.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Device> Devices => Set<Device>();
    public DbSet<FamilyMember> FamilyMembers => Set<FamilyMember>();
    public DbSet<Invitation> Invitations => Set<Invitation>();
    public DbSet<AppNotification> Notifications => Set<AppNotification>();
    public DbSet<ActivityFeedItem> ActivityFeed => Set<ActivityFeedItem>();
    public DbSet<SosAlert> SosAlerts => Set<SosAlert>();
    public DbSet<SosContact> SosContacts => Set<SosContact>();
    public DbSet<OtpRequest> OtpRequests => Set<OtpRequest>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        b.Entity<Device>(e =>
        {
            e.ToTable("devices");
            e.HasKey(x => x.Id);
            e.Property(x => x.DeviceKey).HasMaxLength(200).IsRequired();
            e.HasIndex(x => x.DeviceKey).IsUnique();

            e.OwnsOne(x => x.Profile, p =>
            {
                p.OwnsOne(pp => pp.Permissions);
            });
            e.Navigation(x => x.Profile).IsRequired();

            e.HasMany(x => x.Members).WithOne(m => m.Device)
                .HasForeignKey(m => m.DeviceId).OnDelete(DeleteBehavior.Cascade);
            e.HasMany(x => x.Invitations).WithOne(i => i.Device)
                .HasForeignKey(i => i.DeviceId).OnDelete(DeleteBehavior.Cascade);
            e.HasMany(x => x.Notifications).WithOne(n => n.Device)
                .HasForeignKey(n => n.DeviceId).OnDelete(DeleteBehavior.Cascade);
            e.HasMany(x => x.ActivityFeed).WithOne(a => a.Device)
                .HasForeignKey(a => a.DeviceId).OnDelete(DeleteBehavior.Cascade);
            e.HasMany(x => x.ActiveSosAlerts).WithOne(a => a.Device)
                .HasForeignKey(a => a.DeviceId).OnDelete(DeleteBehavior.Cascade);
            e.HasMany(x => x.SosContacts).WithOne(c => c.Device)
                .HasForeignKey(c => c.DeviceId).OnDelete(DeleteBehavior.Cascade);
            e.HasMany(x => x.OtpRequests).WithOne(o => o.Device)
                .HasForeignKey(o => o.DeviceId).OnDelete(DeleteBehavior.Cascade);
        });

        b.Entity<FamilyMember>(e =>
        {
            e.ToTable("family_members");
            e.HasIndex(x => new { x.DeviceId, x.AppId }).IsUnique();
            e.Property(x => x.Status).HasConversion<string>().HasMaxLength(32);
        });

        b.Entity<Invitation>(e =>
        {
            e.ToTable("invitations");
            e.HasIndex(x => new { x.DeviceId, x.AppId }).IsUnique();
            e.Property(x => x.Status).HasConversion<string>().HasMaxLength(32);
        });

        b.Entity<AppNotification>(e =>
        {
            e.ToTable("notifications");
            e.HasIndex(x => new { x.DeviceId, x.AppId }).IsUnique();
            e.Property(x => x.Category).HasConversion<string>().HasMaxLength(32);
        });

        b.Entity<ActivityFeedItem>(e =>
        {
            e.ToTable("activity_feed");
            e.HasIndex(x => new { x.DeviceId, x.AppId }).IsUnique();
            e.Property(x => x.Severity).HasConversion<string>().HasMaxLength(32);
        });

        b.Entity<SosAlert>(e =>
        {
            e.ToTable("sos_alerts");
        });

        b.Entity<SosContact>(e =>
        {
            e.ToTable("sos_contacts");
            e.Property(x => x.State).HasConversion<string>().HasMaxLength(32);
        });

        b.Entity<OtpRequest>(e =>
        {
            e.ToTable("otp_requests");
            e.Property(x => x.Phone).HasMaxLength(20);
            e.Property(x => x.Code).HasMaxLength(10);
        });
    }
}
