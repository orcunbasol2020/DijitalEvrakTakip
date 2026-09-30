using FluentValidation;

namespace DijitalEvrakTakip.Application.Features.DocumentAllocationRequestFeatures.Commands.CancelAllocationRequests;

public sealed class CancelAllocationRequestsValidator
    : AbstractValidator<CancelAllocationRequestsCommand>
{
    public CancelAllocationRequestsValidator()
    {
        RuleFor(x => x.RequestIds)
            .NotEmpty().WithMessage("İptal edilecek talep seçilmedi")
            .Must(x => x.Count <= AllocationRequestBulkLimits.MaxRequestCount)
            .WithMessage($"Tek seferde en fazla {AllocationRequestBulkLimits.MaxRequestCount} talep işlenebilir");

        RuleFor(x => x.Note)
            .MaximumLength(1000).WithMessage("Gerekçe en fazla 1000 karakter olabilir");
    }
}
