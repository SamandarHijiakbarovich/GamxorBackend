using GamxorOila.Application.Common;
using GamxorOila.Application.Common.Interfaces;
using GamxorOila.Application.Contracts;
using GamxorOila.Application.Mapping;
using GamxorOila.Domain.Entities;
using GamxorOila.Domain.Enums;

namespace GamxorOila.Application.Services;

public class FamilyCareService(
    IDeviceRepository devices,
    IFileStorage files,
    IClock clock,
    IOtpGenerator otp) : IFamilyCareService
{
    private static readonly TimeSpan OtpLifetime = TimeSpan.FromMinutes(10);

    public async Task<BootstrapResponse> BootstrapAsync(string deviceId, CancellationToken ct = default)
    {
        var device = await EnsureDeviceAsync(deviceId, ct);
        device.LastSyncAt = clock.UtcNow;
        Touch(device);
        await devices.SaveChangesAsync(ct);
        return new BootstrapResponse { State = BuildState(device) };
    }

    public async Task<ApiResponseDto> RequestCodeAsync(RequestCodeRequest request, CancellationToken ct = default)
    {
        if (!PhoneNumber.IsComplete(request.Phone))
            return ApiResponseDto.Fail("Telefon raqamni to'liq kiriting.");

        var device = await EnsureDeviceAsync(request.DeviceId, ct);
        var canonical = PhoneNumber.Canonical(request.Phone);

        // Eski faol kodlarni bekor qilamiz
        // (yangi kod yaratishdan oldin) — bu yerda saqlash repo orqali amalga oshadi.
        var code = otp.Generate();
        device.Profile.Phone = PhoneNumber.Pretty(canonical);

        // Shu telefon uchun oldingi ishlatilmagan kodlarni bekor qilamiz.
        foreach (var stale in device.OtpRequests.Where(o => o.Phone == canonical && !o.Consumed))
            stale.Consumed = true;

        device.OtpRequests.Add(new OtpRequest
        {
            DeviceId = device.Id,
            Phone = canonical,
            Code = code,
            ExpiresAt = clock.UtcNow.Add(OtpLifetime)
        });
        Touch(device);
        await devices.SaveChangesAsync(ct);

        var state = BuildState(device) with { OtpRequested = true, OtpHint = code };
        return ApiResponseDto.Ok($"Tasdiqlash kodi yuborildi. Demo kodi: {code}", state);
    }

    public async Task<ApiResponseDto> VerifyCodeAsync(VerifyCodeRequest request, CancellationToken ct = default)
    {
        var device = await EnsureDeviceAsync(request.DeviceId, ct);
        var canonical = PhoneNumber.Canonical(request.Phone);
        var now = clock.UtcNow;

        var match = device.OtpRequests
            .Where(o => o.Phone == canonical)
            .OrderByDescending(o => o.ExpiresAt)
            .FirstOrDefault(o => o.IsValid(request.Code, now));

        if (match is null)
            return ApiResponseDto.Fail("Tasdiqlash kodi noto'g'ri yoki muddati o'tgan.");

        match.Consumed = true;
        device.IsPhoneVerified = true;
        device.IsLoggedIn = true;
        device.Profile.Phone = PhoneNumber.Pretty(canonical);
        UpdateSelf(device, m => m.Phone = device.Profile.Phone);
        Touch(device);
        await devices.SaveChangesAsync(ct);

        var message = device.IsRegistered ? "Hisobingizga kirildi." : "Raqam tasdiqlandi.";
        return ApiResponseDto.Ok(message, BuildState(device));
    }

    public async Task<ApiResponseDto> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.FullName))
            return ApiResponseDto.Fail("Ismingizni kiriting.");

        var device = await EnsureDeviceAsync(request.DeviceId, ct);
        var canonical = PhoneNumber.Canonical(request.Phone);

        device.Profile.FullName = request.FullName.Trim();
        if (canonical.Length == 12) device.Profile.Phone = PhoneNumber.Pretty(canonical);
        device.IsRegistered = true;
        device.IsLoggedIn = true;
        device.IsPhoneVerified = true;

        UpdateSelf(device, m =>
        {
            m.Name = device.Profile.FullName;
            m.Phone = device.Profile.Phone;
        });

        AddNotification(device, "Ro'yxatdan o'tdingiz",
            $"Xush kelibsiz, {StateMapperFirstName(device.Profile.FullName)}! Oila a'zolaringizni taklif qiling.",
            NotificationCategory.System);

        Touch(device);
        await devices.SaveChangesAsync(ct);
        return ApiResponseDto.Ok("Ro'yxatdan o'tish yakunlandi.", BuildState(device));
    }

    public async Task<ApiResponseDto> SaveProfileAsync(SaveProfileRequest request, CancellationToken ct = default)
    {
        var device = await EnsureDeviceAsync(request.DeviceId, ct);
        var p = request.Profile;

        device.Profile.FullName = p.FullName;
        device.Profile.Phone = p.Phone;
        device.Profile.Email = p.Email;
        device.Profile.FamilyLabel = p.FamilyLabel;
        device.Profile.Address = p.Address;
        device.Profile.EmergencyContact = p.EmergencyContact;
        device.Profile.AvatarSeed = p.AvatarSeed;
        device.Profile.Bio = p.Bio;
        device.Profile.AvatarUri = ToRelativeMedia(p.AvatarUri);
        device.Profile.Permissions.LocationEnabled = p.Permissions.LocationEnabled;
        device.Profile.Permissions.MicrophoneEnabled = p.Permissions.MicrophoneEnabled;
        device.Profile.Permissions.NotificationsEnabled = p.Permissions.NotificationsEnabled;
        device.Profile.Permissions.PreciseLocationEnabled = p.Permissions.PreciseLocationEnabled;
        device.Profile.Permissions.BackgroundRefreshEnabled = p.Permissions.BackgroundRefreshEnabled;

        UpdateSelf(device, m =>
        {
            m.Name = p.FullName;
            m.Phone = p.Phone;
            m.Address = p.Address;
            m.AvatarSeed = p.AvatarSeed;
            m.AvatarUri = device.Profile.AvatarUri;
        });

        Touch(device);
        await devices.SaveChangesAsync(ct);
        return ApiResponseDto.Ok("Profil saqlandi.", BuildState(device));
    }

    public async Task<ApiResponseDto> SelectMemberAsync(SelectMemberRequest request, CancellationToken ct = default)
    {
        var device = await EnsureDeviceAsync(request.DeviceId, ct);
        device.SelectedMemberId = request.MemberId;
        Touch(device);
        await devices.SaveChangesAsync(ct);
        return ApiResponseDto.Ok("A'zo tanlandi.", BuildState(device));
    }

    public async Task<ApiResponseDto> RefreshAsync(string deviceId, CancellationToken ct = default)
    {
        var device = await EnsureDeviceAsync(deviceId, ct);
        var now = clock.UtcNow;

        // Jonli tuyg'u uchun harakatdagi a'zolarning ko'rsatkichlarini yangilaymiz.
        foreach (var m in device.Members.Where(m => !m.IsSelf))
        {
            m.LastUpdate = now;
            m.Steps += Random.Shared.Next(5, 120);
            m.Battery = Math.Clamp(m.Battery + Random.Shared.Next(-3, 2), 1, 100);
        }

        device.LastSyncAt = now;
        Touch(device);
        await devices.SaveChangesAsync(ct);
        return ApiResponseDto.Ok("Ma'lumotlar yangilandi.", BuildState(device));
    }

    public async Task<ApiResponseDto> SendInvitationAsync(SendInvitationRequest request, CancellationToken ct = default)
    {
        if (!PhoneNumber.IsComplete(request.Phone))
            return ApiResponseDto.Fail("Taklif uchun telefon raqamni to'liq kiriting.");

        var device = await EnsureDeviceAsync(request.DeviceId, ct);
        var appId = device.NextAppId(device.Invitations.Select(i => i.AppId));

        device.Invitations.Add(new Invitation
        {
            DeviceId = device.Id,
            AppId = appId,
            Name = request.Name.Trim(),
            Relation = request.Relation.Trim(),
            Phone = PhoneNumber.Pretty(request.Phone),
            Status = InvitationStatus.WaitingInstall,
            IsPlatformUser = false,
            CanAccept = false,
            FamilyName = device.Profile.FamilyLabel,
            InvitedByName = "Siz",
            SentAt = clock.UtcNow
        });

        AddNotification(device, "Taklif yuborildi",
            $"{request.Name} ga taklif yuborildi.", NotificationCategory.Invite, inviteAppId: appId);

        Touch(device);
        await devices.SaveChangesAsync(ct);
        return ApiResponseDto.Ok("Taklif yuborildi.", BuildState(device));
    }

    public async Task<ApiResponseDto> AcceptInvitationAsync(string deviceId, int inviteId, CancellationToken ct = default)
    {
        var device = await EnsureDeviceAsync(deviceId, ct);
        var inv = device.Invitations.FirstOrDefault(i => i.AppId == inviteId);
        if (inv is null) return ApiResponseDto.Fail("Taklif topilmadi.", BuildState(device));

        inv.Status = InvitationStatus.Accepted;
        inv.CanAccept = false;

        // Qabul qilingan taklifni oila a'zosiga aylantiramiz.
        var memberAppId = device.NextAppId(device.Members.Select(m => m.AppId));
        device.Members.Add(new FamilyMember
        {
            DeviceId = device.Id,
            AppId = memberAppId,
            MemberCode = "GX-" + Guid.NewGuid().ToString("N")[..6].ToUpperInvariant(),
            Name = inv.Name,
            Relation = inv.Relation,
            Age = 0,
            Address = device.Profile.Address,
            PlaceLabel = "Joylashuv kutilmoqda",
            Battery = 100,
            Status = MemberStatus.Safe,
            Note = "Yangi qo'shilgan a'zo.",
            SafeZone = "Uy",
            Phone = inv.Phone,
            LastUpdate = clock.UtcNow
        });

        AddNotification(device, "Taklif qabul qilindi",
            $"{inv.Name} oilangizga qo'shildi.", NotificationCategory.System);

        Touch(device);
        await devices.SaveChangesAsync(ct);
        return ApiResponseDto.Ok("Taklif qabul qilindi.", BuildState(device));
    }

    public async Task<ApiResponseDto> DismissNotificationAsync(string deviceId, int notificationId, CancellationToken ct = default)
    {
        var device = await EnsureDeviceAsync(deviceId, ct);
        var n = device.Notifications.FirstOrDefault(x => x.AppId == notificationId);
        if (n is null) return ApiResponseDto.Fail("Bildirishnoma topilmadi.", BuildState(device));

        n.IsDismissed = true;
        Touch(device);
        await devices.SaveChangesAsync(ct);
        return ApiResponseDto.Ok("Bildirishnoma o'chirildi.", BuildState(device));
    }

    public async Task<ApiResponseDto> MarkNotificationReadAsync(string deviceId, int notificationId, CancellationToken ct = default)
    {
        var device = await EnsureDeviceAsync(deviceId, ct);
        var n = device.Notifications.FirstOrDefault(x => x.AppId == notificationId);
        if (n is null) return ApiResponseDto.Fail("Bildirishnoma topilmadi.", BuildState(device));

        n.IsRead = true;
        Touch(device);
        await devices.SaveChangesAsync(ct);
        return ApiResponseDto.Ok("O'qildi deb belgilandi.", BuildState(device));
    }

    public async Task<ApiResponseDto> MarkAllNotificationsReadAsync(string deviceId, CancellationToken ct = default)
    {
        var device = await EnsureDeviceAsync(deviceId, ct);
        foreach (var n in device.Notifications) n.IsRead = true;
        Touch(device);
        await devices.SaveChangesAsync(ct);
        return ApiResponseDto.Ok("Barcha bildirishnomalar o'qildi.", BuildState(device));
    }

    public async Task<ApiResponseDto> TriggerSosAsync(string deviceId, CancellationToken ct = default)
    {
        var device = await EnsureDeviceAsync(deviceId, ct);
        var now = clock.UtcNow;

        device.SosContacts.Clear();
        foreach (var m in device.Members.Where(m => !m.IsSelf))
        {
            device.SosContacts.Add(new SosContact
            {
                DeviceId = device.Id,
                MemberAppId = m.AppId,
                Name = m.Name,
                Phone = m.Phone,
                State = SosContactState.Notified
            });
        }

        device.SosIsActive = true;
        device.SosIsSending = false;
        device.SosSentAt = now;
        device.SosSummary = device.SosContacts.Count > 0
            ? $"SOS signali {device.SosContacts.Count} ta oila a'zosiga yuborildi."
            : "SOS signali yuborildi.";

        AddNotification(device, "SOS faollashtirildi",
            device.SosSummary, NotificationCategory.Safety);

        Touch(device);
        await devices.SaveChangesAsync(ct);
        return ApiResponseDto.Ok("SOS signali yuborildi.", BuildState(device));
    }

    public async Task<ApiResponseDto> ClearSosAsync(string deviceId, CancellationToken ct = default)
    {
        var device = await EnsureDeviceAsync(deviceId, ct);
        device.SosIsActive = false;
        device.SosIsSending = false;
        device.SosSummary = string.Empty;
        device.SosSentAt = null;
        device.SosContacts.Clear();
        Touch(device);
        await devices.SaveChangesAsync(ct);
        return ApiResponseDto.Ok("SOS to'xtatildi.", BuildState(device));
    }

    public async Task<ApiResponseDto> SignOutAsync(string deviceId, CancellationToken ct = default)
    {
        var device = await EnsureDeviceAsync(deviceId, ct);
        device.IsLoggedIn = false;
        device.IsRegistered = false;
        device.IsPhoneVerified = false;
        Touch(device);
        await devices.SaveChangesAsync(ct);
        return ApiResponseDto.Ok("Ro'yxatdan chiqildi.", BuildState(device));
    }

    public async Task<UploadResultDto> UploadAssetAsync(
        string deviceId, Stream content, string fileName, string category, string title,
        CancellationToken ct = default)
    {
        var device = await EnsureDeviceAsync(deviceId, ct);
        var url = await files.SaveAsync(content, fileName, category, ct);

        if (category == "profile_avatar")
        {
            device.Profile.AvatarUri = url;
            UpdateSelf(device, m => m.AvatarUri = url);
        }

        Touch(device);
        await devices.SaveChangesAsync(ct);
        return new UploadResultDto { Success = true, Message = "Fayl yuklandi.", FileUrl = url };
    }

    // ---- yordamchilar ----

    private async Task<Device> EnsureDeviceAsync(string deviceId, CancellationToken ct)
    {
        var key = (deviceId ?? string.Empty).Trim();
        if (key.Length == 0) key = "anonymous";

        var device = await devices.GetByKeyAsync(key, ct);
        if (device is not null) return device;

        device = DeviceFactory.CreateSeeded(key, clock.UtcNow);
        await devices.AddAsync(device, ct);
        await devices.SaveChangesAsync(ct);
        return device;
    }

    private AppUiStateDto BuildState(Device device) => StateMapper.ToState(device, clock.UtcNow);

    private void Touch(Device device) => device.UpdatedAt = clock.UtcNow;

    private static void UpdateSelf(Device device, Action<FamilyMember> mutate)
    {
        var self = device.Members.FirstOrDefault(m => m.IsSelf);
        if (self is not null) mutate(self);
    }

    private void AddNotification(Device device, string title, string message,
        NotificationCategory category, int? inviteAppId = null)
    {
        var appId = device.NextAppId(device.Notifications.Select(n => n.AppId));
        device.Notifications.Add(new AppNotification
        {
            DeviceId = device.Id,
            AppId = appId,
            Title = title,
            Message = message,
            Category = category,
            InviteAppId = inviteAppId,
            OccurredAt = clock.UtcNow
        });
    }

    private static string ToRelativeMedia(string value)
    {
        var v = (value ?? string.Empty).Trim();
        if (v.Length == 0) return string.Empty;
        if (v.StartsWith("file:")) return string.Empty;
        if (Uri.TryCreate(v, UriKind.Absolute, out var uri)) return uri.AbsolutePath;
        return v.StartsWith('/') ? v : "/" + v;
    }

    private static string StateMapperFirstName(string fullName)
    {
        var trimmed = (fullName ?? string.Empty).Trim();
        var first = trimmed.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
        return string.IsNullOrEmpty(first) ? "foydalanuvchi" : first;
    }
}
