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
            .WithMessage("Geçersiz Status: 1 (Yeni Kayıt), 2 (Evrak Biriminde), 3 (Teslim Edildi), 4 (Zimmet Devri) veya 5 (Kargoya Verildi) olmalıdır.");
    }
}
