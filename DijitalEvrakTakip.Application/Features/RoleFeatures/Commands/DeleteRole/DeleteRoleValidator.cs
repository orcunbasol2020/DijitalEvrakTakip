using FluentValidation;

namespace DijitalEvrakTakip.Application.Features.RoleFeatures.Commands.DeleteRole;

public sealed class DeleteRoleValidator : AbstractValidator<DeleteRoleCommand>
{
    public DeleteRoleValidator()
    {
        RuleFor(p => p.Id).NotEmpty().WithMessage("Rol Id Boş Olamaz!");
    }
}
