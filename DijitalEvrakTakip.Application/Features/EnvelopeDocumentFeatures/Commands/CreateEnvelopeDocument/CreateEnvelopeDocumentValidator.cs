using FluentValidation;

namespace DijitalEvrakTakip.Application.Features.EnvelopeDocumentFeatures.Commands.CreateEnvelopeDocument;

public sealed class CreateEnvelopeDocumentValidator
    : AbstractValidator<CreateEnvelopeDocumentCommand>
{
    public CreateEnvelopeDocumentValidator()
    {
        RuleFor(p => p.EnvelopeId)
            .NotEmpty().WithMessage("Zarf bilgisi boş olamaz!");

        RuleFor(p => p.QrCode)
            .NotEmpty().WithMessage("Evrak bilgisi boş olamaz!");

        RuleFor(p => p.CreatedUserId)
            .NotEmpty().WithMessage("Kullanıcı bilgisi boş olamaz!");
    }
}