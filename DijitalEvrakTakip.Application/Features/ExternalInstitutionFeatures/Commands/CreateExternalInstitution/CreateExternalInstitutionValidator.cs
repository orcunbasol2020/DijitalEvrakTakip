using FluentValidation;

namespace DijitalEvrakTakip.Application.Features.ExternalInstitutionFeatures.Commands.CreateExternalInstitution;

public sealed class CreateExternalInstitutionValidator
    : AbstractValidator<CreateExternalInstitutionCommand>
{
    public CreateExternalInstitutionValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty().WithMessage("Kurum adı boş olamaz!")
            .MaximumLength(250).WithMessage("Kurum adı 250 karakterden fazla olamaz!");
    }
}
