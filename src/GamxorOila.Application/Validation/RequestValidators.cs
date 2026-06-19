using FluentValidation;
using GamxorOila.Application.Common;
using GamxorOila.Application.Contracts;

namespace GamxorOila.Application.Validation;

public class RequestCodeRequestValidator : AbstractValidator<RequestCodeRequest>
{
    public RequestCodeRequestValidator()
    {
        RuleFor(x => x.Phone)
            .Must(PhoneNumber.IsComplete)
            .WithMessage("Telefon raqamni to'liq kiriting.");
    }
}

public class VerifyCodeRequestValidator : AbstractValidator<VerifyCodeRequest>
{
    public VerifyCodeRequestValidator()
    {
        RuleFor(x => x.Phone)
            .Must(PhoneNumber.IsComplete)
            .WithMessage("Telefon raqamni to'liq kiriting.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Tasdiqlash kodini kiriting.")
            .Matches(@"^\d{4,6}$").WithMessage("Kod 4-6 raqamdan iborat bo'lishi kerak.");
    }
}

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Ismingizni kiriting.")
            .MinimumLength(2).WithMessage("Ism juda qisqa.")
            .MaximumLength(80);

        RuleFor(x => x.Phone)
            .Must(PhoneNumber.IsComplete)
            .WithMessage("Telefon raqamni to'liq kiriting.");
    }
}

public class SendInvitationRequestValidator : AbstractValidator<SendInvitationRequest>
{
    public SendInvitationRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Taklif uchun ism kiriting.")
            .MaximumLength(80);

        RuleFor(x => x.Phone)
            .Must(PhoneNumber.IsComplete)
            .WithMessage("Taklif uchun telefon raqamni to'liq kiriting.");
    }
}

public class SaveProfileRequestValidator : AbstractValidator<SaveProfileRequest>
{
    public SaveProfileRequestValidator()
    {
        RuleFor(x => x.Profile).NotNull().WithMessage("Profil ma'lumotlari yo'q.");
        RuleFor(x => x.Profile.FullName)
            .NotEmpty().WithMessage("Ism bo'sh bo'lmasligi kerak.")
            .MaximumLength(80);
    }
}
