using FluentValidation;

namespace DijitalEvrakTakip.Application.Features.IncomingDocumentAttachmentFeatures.Commands.RemoveIncomingDocumentAttachment;

public sealed class RemoveIncomingDocumentAttachmentValidator : AbstractValidator<RemoveIncomingDocumentAttachmentCommand>
{
    public RemoveIncomingDocumentAttachmentValidator()
    {
        RuleFor(p => p.Id)
            .NotEmpty().WithMessage("Ek bilgisi boş geçilemez");

        RuleFor(p => p.UserId)
            .NotEmpty().WithMessage("Kullanıcı bilgisi boş geçilemez");
    }
}
