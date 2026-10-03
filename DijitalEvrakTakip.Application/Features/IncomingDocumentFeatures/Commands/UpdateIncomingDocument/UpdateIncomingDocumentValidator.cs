using FluentValidation;

namespace DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Commands.UpdateIncomingDocument;

public sealed class UpdateIncomingDocumentValidator : AbstractValidator<UpdateIncomingDocumentCommand>
{
    public UpdateIncomingDocumentValidator()
    {
        RuleFor(p => p.AttachmentDescription)
            .MaximumLength(1000).WithMessage("Ek açıklaması en fazla 1000 karakter olabilir");
    }
}
