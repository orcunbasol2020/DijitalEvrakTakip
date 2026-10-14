using System.Text;
using FluentValidation;

namespace DijitalEvrakTakip.Application.Features.IncomingDocumentAttachmentFeatures.Commands.UploadIncomingDocumentAttachment;

public sealed class UploadIncomingDocumentAttachmentValidator : AbstractValidator<UploadIncomingDocumentAttachmentCommand>
{
    public const long MaxFileSizeBytes = 10 * 1024 * 1024;

    private static readonly byte[] PdfSignature = Encoding.ASCII.GetBytes("%PDF-");

    public UploadIncomingDocumentAttachmentValidator()
    {
        RuleFor(p => p.IncomingDocumentId)
            .NotEmpty().WithMessage("Evrak bilgisi boş geçilemez");

        RuleFor(p => p.UserId)
            .NotEmpty().WithMessage("Kullanıcı bilgisi boş geçilemez");

        RuleFor(p => p.FileName)
            .NotEmpty().WithMessage("Dosya adı boş olamaz!")
            .MaximumLength(260).WithMessage("Dosya adı en fazla 260 karakter olabilir")
            .Must(fileName => string.Equals(
                Path.GetExtension(fileName), ".pdf", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Desteklenmeyen dosya türü. Yalnızca PDF yüklenebilir.");

        RuleFor(p => p.FileContent)
            .NotEmpty().WithMessage("Yüklenecek dosya boş olamaz!")
            .Must(content => content.Length <= MaxFileSizeBytes)
            .WithMessage("Ek dosyası 10 MB'ı geçemez!")
            // Uzantısı .pdf yapılmış başka dosyalar EYP paketine girmesin
            .Must(content => content.AsSpan().StartsWith(PdfSignature))
            .WithMessage("Dosya geçerli bir PDF değil.");

        RuleFor(p => p.Name)
            .MaximumLength(250).WithMessage("Ek adı en fazla 250 karakter olabilir");

        RuleFor(p => p.Description)
            .MaximumLength(500).WithMessage("Ek açıklaması en fazla 500 karakter olabilir");
    }
}
