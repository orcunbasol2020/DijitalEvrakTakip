using FluentValidation;

namespace DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Commands.UploadIncomingDocumentFile;

public sealed class UploadIncomingDocumentFileValidator : AbstractValidator<UploadIncomingDocumentFileCommand>
{
    private const long MaxFileSizeBytes = 20 * 1024 * 1024;

    public UploadIncomingDocumentFileValidator()
    {
        RuleFor(p => p.IncomingDocumentId)
            .NotEmpty().WithMessage("Evrak bilgisi boş geçilemez");

        RuleFor(p => p.UserId)
            .NotEmpty().WithMessage("Kullanıcı bilgisi boş geçilemez");

        RuleFor(p => p.FileName)
            .NotEmpty().WithMessage("Dosya adı boş olamaz!")
            .Must(fileName => string.Equals(
                Path.GetExtension(fileName), ".pdf", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Desteklenmeyen dosya türü. Yalnızca PDF yüklenebilir.");

        RuleFor(p => p.FileContent)
            .NotEmpty().WithMessage("Yüklenecek dosya boş olamaz!")
            .Must(content => content.Length <= MaxFileSizeBytes)
            .WithMessage("Dosya boyutu 20 MB'ı geçemez!");
    }
}
