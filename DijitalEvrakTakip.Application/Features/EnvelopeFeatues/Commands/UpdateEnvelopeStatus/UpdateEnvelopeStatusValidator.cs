using DijitalEvrakTakip.Domain.Enums;
using FluentValidation;

namespace DijitalEvrakTakip.Application.Features.EnvelopeFeatures.Commands.UpdateEnvelopeStatus;

public sealed class UpdateEnvelopeStatusValidator
    : AbstractValidator<UpdateEnvelopeStatusCommand>
{
    public UpdateEnvelopeStatusValidator()
    {
        RuleFor(p => p.Id)
            .NotEmpty().WithMessage("Zarf Id boş olamaz!");

        RuleFor(p => p.Status)
            .Must(status => Enum.IsDefined(typeof(EnvelopeStatusEnum), status))
            .WithMessage("Geçersiz Status: 1 (Oluşturuldu) veya 2 (Teslim Edildi) olmalıdır.");
    }
}
