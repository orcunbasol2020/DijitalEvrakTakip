using FluentValidation;

namespace DijitalEvrakTakip.Application.Features.EnvelopeFeatures.Commands.CreateEnvelope;

public sealed class CreateEnvelopeValidator
    : AbstractValidator<CreateEnvelopeCommand>
{
    public CreateEnvelopeValidator()
    {
        RuleFor(p => p.CreatedByUserId)
            .NotEmpty().WithMessage("Kullanıcı bilgisi boş olamaz!");
    }
}