using FluentValidation;

namespace DijitalEvrakTakip.Application.Features.ExternalUserFeatures.Commands.UpdateExternalUser;

public sealed class UpdateExternalUserValidator : AbstractValidator<UpdateExternalUserCommand>
{
    public UpdateExternalUserValidator()
    {
        RuleFor(p => p.Id)
            .NotEmpty().WithMessage("Harici kullanıcı Id boş olamaz!");

        RuleFor(p => p.Name)
            .MinimumLength(3).WithMessage("Ad en az 3 karakter olmalıdır!")
            .MaximumLength(200).WithMessage("Ad 200 karakterden fazla olamaz!")
            .When(p => p.Name is not null);

        RuleFor(p => p.Surname)
            .MinimumLength(3).WithMessage("Soyad en az 3 karakter olmalıdır!")
            .MaximumLength(200).WithMessage("Soyad 200 karakterden fazla olamaz!")
            .When(p => p.Surname is not null);

        RuleFor(p => p.Email)
            .EmailAddress().WithMessage("Geçerli bir eposta adresi gönderiniz")
            .When(p => p.Email is not null);
    }
}
