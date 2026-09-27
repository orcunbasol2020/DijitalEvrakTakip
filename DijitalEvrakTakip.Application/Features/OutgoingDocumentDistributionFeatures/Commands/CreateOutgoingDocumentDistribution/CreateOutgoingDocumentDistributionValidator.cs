using FluentValidation;

namespace DijitalEvrakTakip.Application.Features.OutgoingDocumentDistributionFeatures.Commands.CreateOutgoingDocumentDistribution;

public sealed class CreateOutgoingDocumentDistributionValidator
    : AbstractValidator<CreateOutgoingDocumentDistributionCommand>
{
    public CreateOutgoingDocumentDistributionValidator()
    {
        RuleFor(p => p.OutgoingDocumentId)
            .NotEmpty().WithMessage("Evrak bilgisi boş olamaz!");

        RuleFor(p => p.Recipients)
            .NotEmpty().WithMessage("En az bir alıcı girilmelidir!");

        RuleForEach(p => p.Recipients)
            .Must(r => r.DepartmentId.HasValue ^ r.ExternalInstitutionId.HasValue)
            .WithMessage("Her alıcı için birim veya dış kurumdan yalnızca biri seçilmelidir!");
    }
}
