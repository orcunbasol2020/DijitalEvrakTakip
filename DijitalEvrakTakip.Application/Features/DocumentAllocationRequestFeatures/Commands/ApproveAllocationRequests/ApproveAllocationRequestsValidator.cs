using FluentValidation;

namespace DijitalEvrakTakip.Application.Features.DocumentAllocationRequestFeatures.Commands.ApproveAllocationRequests;

public sealed class ApproveAllocationRequestsValidator
    : AbstractValidator<ApproveAllocationRequestsCommand>
{
    public ApproveAllocationRequestsValidator()
    {
        RuleFor(x => x.RequestIds)
            .NotEmpty().WithMessage("Onaylanacak talep seçilmedi")
            .Must(x => x.Count <= AllocationRequestBulkLimits.MaxRequestCount)
            .WithMessage($"Tek seferde en fazla {AllocationRequestBulkLimits.MaxRequestCount} talep işlenebilir");
    }
}
