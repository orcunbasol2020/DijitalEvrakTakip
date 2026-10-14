using FluentValidation;

namespace DijitalEvrakTakip.Application.Features.IncomingDocumentAttachmentFeatures.Commands.UpdateIncomingDocumentAttachment;

public sealed class UpdateIncomingDocumentAttachmentValidator : AbstractValidator<UpdateIncomingDocumentAttachmentCommand>
{
    public UpdateIncomingDocumentAttachmentValidator()
    {
        RuleFor(p => p.Id)
            .NotEmpty().WithMessage("Ek bilgisi boş geçilemez");

        RuleFor(p => p.Name)
            .Must(name => name is null || !string.IsNullOrWhiteSpace(name))
            .WithMessage("Ek adı boş olamaz!")
            .MaximumLength(250).WithMessage("Ek adı en fazla 250 karakter olabilir");

        RuleFor(p => p.Description)
            .MaximumLength(500).WithMessage("Ek açıklaması en fazla 500 karakter olabilir");

        RuleFor(p => p.OrderNo)
            .GreaterThan(0).When(p => p.OrderNo.HasValue)
            .WithMessage("Ek sırası 1 veya daha büyük olmalıdır");
    }
}
