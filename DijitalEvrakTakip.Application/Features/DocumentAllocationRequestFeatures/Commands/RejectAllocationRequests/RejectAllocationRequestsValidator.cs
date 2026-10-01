using FluentValidation;

namespace DijitalEvrakTakip.Application.Features.DocumentAllocationRequestFeatures.Commands.RejectAllocationRequests;

public sealed class RejectAllocationRequestsValidator
    : AbstractValidator<RejectAllocationRequestsCommand>
{
    public RejectAllocationRequestsValidator()
    {
        RuleFor(x => x.RequestIds)
            .NotEmpty().WithMessage("Reddedilecek talep seçilmedi")
            .Must(x => x.Count <= AllocationRequestBulkLimits.MaxRequestCount)
            .WithMessage($"Tek seferde en fazla {AllocationRequestBulkLimits.MaxRequestCount} talep işlenebilir");

        RuleFor(x => x.Note)
            .MaximumLength(1000).WithMessage("Gerekçe en fazla 1000 karakter olabilir");
    }
}
