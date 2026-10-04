using FluentValidation;

namespace DijitalEvrakTakip.Application.Features.DocumentAllocationRequestFeatures.Commands.ApproveAllocationRequest;

public sealed class ApproveAllocationRequestValidator
    : AbstractValidator<ApproveAllocationRequestCommand>
{
    public ApproveAllocationRequestValidator()
    {
        RuleFor(x => x.Note)
            .NotEmpty().When(x => x.HasDiscrepancy).WithMessage("Şerhli kabulde şerh açıklaması girilmelidir")
            .MaximumLength(1000).WithMessage("Açıklama en fazla 1000 karakter olabilir");
    }
}
