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

        RuleFor(p => p.DetsisCode)
            .Matches(@"^\d{8}$").WithMessage("DETSİS kodu 8 haneli sayısal bir değer olmalıdır!")
            .When(p => p.DetsisCode is not null);
    }
}
