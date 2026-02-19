using FluentValidation;

namespace DijitalEvrakTakip.Application.Features.RoleFeatures.Commands.CreateRole;

public sealed class CreateRoleValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleValidator()
    {
        RuleFor(p => p.Name).NotEmpty().WithMessage("Role Adı Boş Olamaz!");

        RuleFor(p => p.IsActive).NotNull().WithMessage("Aktif Durumu Boş Olamaz!");
    }
}
