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

        RuleFor(p => p.DetsisCode)
            .Matches(@"^\d{8}$").WithMessage("DETSİS kodu 8 haneli sayısal bir değer olmalıdır!")
            .When(p => p.DetsisCode is not null);
    }
}
