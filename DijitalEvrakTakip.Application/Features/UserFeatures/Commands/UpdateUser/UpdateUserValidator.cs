using FluentValidation;

namespace DijitalEvrakTakip.Application.Features.UserFeatures.Commands.UpdateUser;

public sealed class UpdateUserValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserValidator()
    {
        RuleFor(p => p.Id)
            .NotEmpty().WithMessage("Kullanıcı Id boş olamaz!");

        RuleFor(p => p.Name)
            .MinimumLength(3).WithMessage("Kullanıcı adı en az 3 karakter olmalıdır!")
            .MaximumLength(200).WithMessage("Kullanıcı adı 200 karakterden fazla olamaz!")
            .When(p => p.Name is not null);

        RuleFor(p => p.Surname)
            .MinimumLength(3).WithMessage("Kullanıcı soyadı en az 3 karakter olmalıdır!")
            .MaximumLength(200).WithMessage("Kullanıcı soyadı 200 karakterden fazla olamaz!")
            .When(p => p.Surname is not null);

        RuleFor(p => p.Email)
            .EmailAddress().WithMessage("Geçerli bir eposta adresi gönderiniz")
            .When(p => p.Email is not null);
    }
}
