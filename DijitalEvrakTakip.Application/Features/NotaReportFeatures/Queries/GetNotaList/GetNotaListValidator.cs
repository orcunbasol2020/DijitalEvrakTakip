using FluentValidation;

namespace DijitalEvrakTakip.Application.Features.NotaReportFeatures.Queries.GetNotaList;

public sealed class GetNotaListValidator : AbstractValidator<GetNotaListQuery>
{
    public GetNotaListValidator()
    {
        RuleFor(x => x.ExternalInstitutionId)
            .NotEmpty().WithMessage("Misyon seçilmelidir");

        RuleFor(x => x.Year)
            .InclusiveBetween(1900, 2999).When(x => x.Year.HasValue)
            .WithMessage("Yıl geçersiz");

        RuleFor(x => x.Month)
            .InclusiveBetween(1, 12).When(x => x.Month.HasValue)
            .WithMessage("Ay 1 ile 12 arasında olmalıdır");

        RuleFor(x => x.DateField)
            .Must(NotaReportPeriodRules.IsValidDateField)
            .WithMessage("dateField 'created' ya da 'document' olmalıdır");
    }
}
