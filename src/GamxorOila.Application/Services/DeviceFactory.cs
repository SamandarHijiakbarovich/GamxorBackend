using GamxorOila.Application.Common;
using GamxorOila.Domain.Entities;
using GamxorOila.Domain.Enums;
using GamxorOila.Domain.ValueObjects;

namespace GamxorOila.Application.Services;

/// <summary>Yangi qurilma uchun namunaviy oila ma'lumotlari bilan Device yaratadi.</summary>
public static class DeviceFactory
{
    public static Device CreateSeeded(string deviceKey, DateTimeOffset now)
    {
        var device = new Device
        {
            DeviceKey = deviceKey,
            IsRegistered = false,
            IsLoggedIn = false,
            IsPhoneVerified = false,
            LastSyncAt = now,
            SelectedMemberId = 0,
            TrustedPlacesCount = 3,
            NextCheckInLabel = "20:00 oilaviy check-in",
            Profile = new CaregiverProfile
            {
                FullName = "Yangi foydalanuvchi",
                Phone = "+998 ",
                FamilyLabel = "Mening oilam",
                Address = "Toshkent, Chilonzor",
                EmergencyContact = "+998 71 100 00 00",
                MemberCode = MemberCode()
            }
        };

        // Foydalanuvchining o'zi (selfMember, AppId = 0)
        device.Members.Add(new FamilyMember
        {
            DeviceId = device.Id,
            AppId = 0,
            IsSelf = true,
            MemberCode = "SELF",
            Name = "Yangi foydalanuvchi",
            Relation = "Men",
            Age = 31,
            Lat = 41.3111,
            Lon = 69.2797,
            Address = "Toshkent, Chilonzor",
            PlaceLabel = "Mening joylashuvim",
            Battery = 92,
            Steps = 4680,
            HeartRate = 76,
            Status = MemberStatus.Safe,
            Note = "Qurilma faol holatda.",
            SafeZone = "Uy va ish yo'nalishi",
            Schedule = "20:00 oilaviy check-in",
            LastUpdate = now
        });

        device.Members.AddRange(
        [
            new FamilyMember
            {
                DeviceId = device.Id, AppId = 1, MemberCode = MemberCode(),
                Name = "Dilnoza opa", Relation = "Onam", Age = 58,
                Lat = 41.3275, Lon = 69.2817, Address = "Toshkent, Yunusobod",
                PlaceLabel = "Uy", Battery = 78, Steps = 2310, HeartRate = 82,
                Status = MemberStatus.Safe, Note = "Uyda, xavfsiz hududda.",
                SafeZone = "Uy", Phone = "+998 90 111 22 33", Schedule = "Tushlik 13:00",
                AvatarSeed = 11, DistanceKm = 2.4, LastUpdate = now.AddMinutes(-3)
            },
            new FamilyMember
            {
                DeviceId = device.Id, AppId = 2, MemberCode = MemberCode(),
                Name = "Akmal aka", Relation = "Otam", Age = 61,
                Lat = 41.2995, Lon = 69.2401, Address = "Toshkent, Olmazor",
                PlaceLabel = "Bog'", Battery = 54, Steps = 6120, HeartRate = 88,
                Status = MemberStatus.Moving, Note = "Harakatda — bog' tomon ketmoqda.",
                SafeZone = "Bog' va uy", Phone = "+998 90 444 55 66", Schedule = "Kechki sayr 18:30",
                AvatarSeed = 22, DistanceKm = 5.1, LastUpdate = now.AddMinutes(-12)
            },
            new FamilyMember
            {
                DeviceId = device.Id, AppId = 3, MemberCode = MemberCode(),
                Name = "Madina", Relation = "Singlim", Age = 17,
                Lat = 41.3380, Lon = 69.3340, Address = "Toshkent, Mirzo Ulug'bek",
                PlaceLabel = "Maktab", Battery = 23, Steps = 9870, HeartRate = 105,
                Status = MemberStatus.NeedsAttention, Note = "Batareya kam va belgilangan hududdan tashqarida.",
                SafeZone = "Maktab", Phone = "+998 90 777 88 99", Schedule = "Darslar 08:00-14:00",
                AvatarSeed = 33, DistanceKm = 7.8, LastUpdate = now.AddMinutes(-25)
            }
        ]);

        device.ActivityFeed.AddRange(
        [
            new ActivityFeedItem
            {
                DeviceId = device.Id, AppId = 1, MemberAppId = 1,
                Title = "Dilnoza opa uyga keldi", Subtitle = "Xavfsiz hududga kirdi",
                Severity = FeedSeverity.Positive, OccurredAt = now.AddMinutes(-3)
            },
            new ActivityFeedItem
            {
                DeviceId = device.Id, AppId = 2, MemberAppId = 2,
                Title = "Akmal aka harakatda", Subtitle = "Olmazor tomon yo'lda",
                Severity = FeedSeverity.Neutral, OccurredAt = now.AddMinutes(-12)
            },
            new ActivityFeedItem
            {
                DeviceId = device.Id, AppId = 3, MemberAppId = 3,
                Title = "Madina diqqat talab qiladi", Subtitle = "Batareya 23% va hududdan tashqarida",
                Severity = FeedSeverity.Warning, OccurredAt = now.AddMinutes(-25)
            }
        ]);

        device.Notifications.AddRange(
        [
            new AppNotification
            {
                DeviceId = device.Id, AppId = 1, Title = "Xush kelibsiz!",
                Message = "Family Care hisobingiz tayyor. Oila a'zolaringizni taklif qiling.",
                Category = NotificationCategory.System, OccurredAt = now.AddMinutes(-1)
            },
            new AppNotification
            {
                DeviceId = device.Id, AppId = 2, Title = "Xavfsizlik ogohlantirishi",
                Message = "Madina belgilangan xavfsiz hududdan chiqdi.",
                Category = NotificationCategory.Safety, OccurredAt = now.AddMinutes(-25)
            }
        ]);

        device.Invitations.Add(new Invitation
        {
            DeviceId = device.Id, AppId = 1, Name = "Bobur", Relation = "Akam",
            Phone = "+998 93 222 11 00", Status = InvitationStatus.WaitingInstall,
            IsPlatformUser = false, CanAccept = false, FamilyName = "Mening oilam",
            InvitedByName = "Siz", SentAt = now.AddHours(-2)
        });

        return device;
    }

    private static string MemberCode() =>
        "GX-" + Guid.NewGuid().ToString("N")[..6].ToUpperInvariant();
}
