using FluentValidation;

namespace DijitalEvrakTakip.Application.Features.DocumentAllocationRequestFeatures.Commands.CancelAllocationRequest;

public sealed class CancelAllocationRequestValidator
    : AbstractValidator<CancelAllocationRequestCommand>
{
    public CancelAllocationRequestValidator()
    {
        RuleFor(x => x.Note)
            .MaximumLength(1000).WithMessage("Gerekçe en fazla 1000 karakter olabilir");
    }
}
