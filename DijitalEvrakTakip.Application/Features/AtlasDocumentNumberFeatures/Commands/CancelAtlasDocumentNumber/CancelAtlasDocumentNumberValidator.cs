using FluentValidation;

namespace DijitalEvrakTakip.Application.Features.AtlasDocumentNumberFeatures.Commands.CancelAtlasDocumentNumber;

public sealed class CancelAtlasDocumentNumberValidator : AbstractValidator<CancelAtlasDocumentNumberCommand>
{
    public CancelAtlasDocumentNumberValidator()
    {
        RuleFor(p => p.QrCode)
            .NotEmpty().WithMessage("Evrak numarası boş geçilemez");

        RuleFor(p => p.Reason)
            .MaximumLength(500).WithMessage("İptal nedeni en fazla 500 karakter olabilir");
    }
}
