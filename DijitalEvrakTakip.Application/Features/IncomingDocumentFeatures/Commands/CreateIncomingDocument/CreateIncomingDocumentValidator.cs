using FluentValidation;

namespace DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Commands.CreateIncomingDocument;

public sealed class CreateIncomingDocumentValidator : AbstractValidator<CreateIncomingDocumentCommand>
{
    public CreateIncomingDocumentValidator()
    {
        RuleFor(p => p.AttachmentDescription)
            .MaximumLength(1000).WithMessage("Ek açıklaması en fazla 1000 karakter olabilir");

        RuleFor(p => p.Recipients!)
            .SetValidator(new IncomingDocumentRecipientsValidator())
            .When(p => p.Recipients is not null);
    }
}
