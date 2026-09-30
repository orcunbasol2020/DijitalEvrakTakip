using FluentValidation;

namespace DijitalEvrakTakip.Application.Features.DocumentAllocationRequestFeatures.Commands.RejectAllocationRequest;

public sealed class RejectAllocationRequestValidator
    : AbstractValidator<RejectAllocationRequestCommand>
{
    public RejectAllocationRequestValidator()
    {
        RuleFor(x => x.Note)
            .MaximumLength(1000).WithMessage("Gerekçe en fazla 1000 karakter olabilir");
    }
}
