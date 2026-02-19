using FluentValidation;

namespace DijitalEvrakTakip.Application.Features.UserFeatures.Commands.CreateUser;

public sealed class CreateUserValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserValidator()
    {
        RuleFor(p => p.Name).NotEmpty().WithMessage("Kullanıcı adı boş olamaz!");
        RuleFor(p => p.Name).NotNull().WithMessage("Kullanıcı adı boş olamaz!");
        RuleFor(p => p.Name).MinimumLength(3).WithMessage("Kullanıcı adı boş olamaz!");
        RuleFor(p => p.Name).MaximumLength(200).WithMessage("Kullanıcı adı 200 karakterden fazla olamaz!");

        RuleFor(p => p.Surname).NotEmpty().WithMessage("Kullanıcı adı boş olamaz!");
        RuleFor(p => p.Surname).NotNull().WithMessage("Kullanıcı adı boş olamaz!");
        RuleFor(p => p.Surname).MinimumLength(3).WithMessage("Kullanıcı adı boş olamaz!");
        RuleFor(p => p.Surname).MaximumLength(200).WithMessage("Kullanıcı adı 200 karakterden fazla olamaz!");

        RuleFor(p => p.DepartmentId).NotEmpty().WithMessage("Birim bilgisi boş geçilemez");
        RuleFor(p => p.DepartmentId).NotNull().WithMessage("Birim bilgisi boş geçilemez");

        RuleFor(p => p.Email).NotEmpty().WithMessage("Eposta bilgisi boş geçilemez");
        RuleFor(p => p.Email).NotNull().WithMessage("Eposta bilgisi boş geçilemez");
        RuleFor(p => p.Email).EmailAddress().WithMessage("Geçerli bir eposta adresi gönderiniz");

        RuleFor(p => p.UserName).NotEmpty().WithMessage("Kullanıcı adı bilgisi boş geçilemez");
        RuleFor(p => p.UserName).NotNull().WithMessage("Kullanıcı adı bilgisi boş geçilemez");

        RuleFor(p => p.IsActive).NotEmpty().WithMessage("Aktif bilgisi boş geçilemez");
        RuleFor(p => p.IsActive).NotNull().WithMessage("Aktif bilgisi boş geçilemez");

    }
}
