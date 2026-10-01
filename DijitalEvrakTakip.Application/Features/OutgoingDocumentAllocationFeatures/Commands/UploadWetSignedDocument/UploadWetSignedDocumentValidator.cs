using FluentValidation;

namespace DijitalEvrakTakip.Application.Features.OutgoingDocumentAllocationFeatures.Commands.UploadWetSignedDocument;

public sealed class UploadWetSignedDocumentValidator : AbstractValidator<UploadWetSignedDocumentCommand>
{
    private static readonly string[] AllowedExtensions =
        { ".pdf", ".jpg", ".jpeg", ".png", ".tif", ".tiff" };

    private const long MaxFileSizeBytes = 20 * 1024 * 1024;

    public UploadWetSignedDocumentValidator()
    {
        RuleFor(p => p.OutgoingDocumentId)
            .NotEmpty().WithMessage("Evrak bilgisi boş geçilemez");

        RuleFor(p => p.FileName)
            .NotEmpty().WithMessage("Dosya adı boş olamaz!")
            .Must(fileName => AllowedExtensions.Contains(
                Path.GetExtension(fileName), StringComparer.OrdinalIgnoreCase))
            .WithMessage("Desteklenmeyen dosya türü. İzin verilen türler: " +
                string.Join(", ", AllowedExtensions));

        RuleFor(p => p.FileContent)
            .NotEmpty().WithMessage("Yüklenecek dosya boş olamaz!")
            .Must(content => content.Length <= MaxFileSizeBytes)
            .WithMessage("Dosya boyutu 20 MB'ı geçemez!");
    }
}
