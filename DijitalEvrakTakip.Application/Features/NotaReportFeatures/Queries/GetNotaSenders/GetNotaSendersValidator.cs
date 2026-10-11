using FluentValidation;

namespace DijitalEvrakTakip.Application.Features.NotaReportFeatures.Queries.GetNotaSenders;

public sealed class GetNotaSendersValidator : AbstractValidator<GetNotaSendersQuery>
{
    public GetNotaSendersValidator()
    {
        RuleFor(x => x.Year)
            .InclusiveBetween(1900, 2999).When(x => x.Year.HasValue)
            .WithMessage("Yıl geçersiz");

        RuleFor(x => x.DateField)
            .Must(NotaReportPeriodRules.IsValidDateField)
            .WithMessage("dateField 'created' ya da 'document' olmalıdır");
    }
}
