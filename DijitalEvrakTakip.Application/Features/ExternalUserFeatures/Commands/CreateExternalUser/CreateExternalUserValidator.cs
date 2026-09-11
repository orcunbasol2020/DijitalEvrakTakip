using FluentValidation;

namespace DijitalEvrakTakip.Application.Features.ExternalUserFeatures.Commands.CreateExternalUser;

public sealed class CreateExternalUserValidator : AbstractValidator<CreateExternalUserCommand>
{
    public CreateExternalUserValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty().WithMessage("Ad boş olamaz!")
            .MinimumLength(3).WithMessage("Ad en az 3 karakter olmalıdır!")
            .MaximumLength(200).WithMessage("Ad 200 karakterden fazla olamaz!");

        RuleFor(p => p.Surname)
            .NotEmpty().WithMessage("Soyad boş olamaz!")
            .MinimumLength(3).WithMessage("Soyad en az 3 karakter olmalıdır!")
            .MaximumLength(200).WithMessage("Soyad 200 karakterden fazla olamaz!");

        RuleFor(p => p.Email)
            .NotEmpty().WithMessage("Eposta bilgisi boş geçilemez")
            .EmailAddress().WithMessage("Geçerli bir eposta adresi gönderiniz");

        RuleFor(p => p.IdentityNo)
            .NotEmpty().WithMessage("Kimlik numarası boş olamaz!");

        RuleFor(p => p.ExternalInstitutionId)
            .NotEmpty().WithMessage("Kurum bilgisi boş geçilemez");
    }
}
