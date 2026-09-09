using FluentValidation;

namespace DijitalEvrakTakip.Application.Features.ExternalInstitutionFeatures.Commands.UpdateExternalInstitution;

public sealed class UpdateExternalInstitutionValidator
    : AbstractValidator<UpdateExternalInstitutionCommand>
{
    public UpdateExternalInstitutionValidator()
    {
        RuleFor(p => p.Id)
            .NotEmpty().WithMessage("Kurum Id boş olamaz!");

        RuleFor(p => p.Name)
            .MaximumLength(250).WithMessage("Kurum adı 250 karakterden fazla olamaz!")
            .When(p => p.Name is not null);
    }
}
