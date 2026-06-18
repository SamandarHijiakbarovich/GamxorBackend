namespace GamxorOila.Domain.ValueObjects;

/// <summary>Asosiy parvarishchi (caregiver) profili — Device agregatining bir qismi.</summary>
public class CaregiverProfile
{
    public string FullName { get; set; } = "Yangi foydalanuvchi";
    public string Phone { get; set; } = "+998 ";
    public string Email { get; set; } = string.Empty;
    public string FamilyLabel { get; set; } = "Mening oilam";
    public string Address { get; set; } = "Toshkent";
    public string EmergencyContact { get; set; } = "+998 ";
    public int AvatarSeed { get; set; }

    /// <summary>Yuklangan avatar uchun nisbiy yo'l (masalan, /media/uploads/...).</summary>
    public string AvatarUri { get; set; } = string.Empty;

    public string Bio { get; set; } =
        "Oilaviy xavfsizlik va joylashuv nazorati shu yerdan boshqariladi.";

    public string MemberCode { get; set; } = string.Empty;

    public ProfilePermissions Permissions { get; set; } = new();
}
