using FluentValidation;

namespace DijitalEvrakTakip.Application.Features.EnvelopeFeatures.Commands.UpdateEnvelope;

public sealed class UpdateEnvelopeValidator : AbstractValidator<UpdateEnvelopeCommand>
{
    public UpdateEnvelopeValidator()
    {
        RuleFor(p => p.Id)
            .NotEmpty().WithMessage("Zarf Id boş olamaz!");

        RuleFor(p => p.UnitName)
            .MaximumLength(250).WithMessage("Birim adı en fazla 250 karakter olabilir.");

        RuleFor(p => p.Address)
            .MaximumLength(1000).WithMessage("Adres en fazla 1000 karakter olabilir.");
    }
}
