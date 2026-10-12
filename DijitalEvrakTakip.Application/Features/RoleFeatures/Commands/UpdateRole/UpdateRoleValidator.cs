using FluentValidation;

namespace DijitalEvrakTakip.Application.Features.RoleFeatures.Commands.UpdateRole;

public sealed class UpdateRoleValidator : AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleValidator()
    {
        RuleFor(p => p.Id).NotEmpty().WithMessage("Rol Id Boş Olamaz!");

        RuleFor(p => p.Name).NotEmpty().WithMessage("Rol Adı Boş Olamaz!");
    }
}
