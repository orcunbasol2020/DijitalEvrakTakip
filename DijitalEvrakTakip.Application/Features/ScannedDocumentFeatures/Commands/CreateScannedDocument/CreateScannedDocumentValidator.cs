using FluentValidation;

namespace DijitalEvrakTakip.Application.Features.ScannedDocumentFeatures.Commands.CreateScannedDocument;

public sealed class CreateScannedDocumentValidator
    : AbstractValidator<CreateScannedDocumentCommand>
{
    public CreateScannedDocumentValidator()
    {
        RuleFor(p => p.FileName)
            .NotEmpty().WithMessage("Dosya adı boş olamaz!")
            .MaximumLength(200).WithMessage("Dosya adı 200 karakterden fazla olamaz!");

    }
}