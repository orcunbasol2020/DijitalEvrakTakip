using FluentValidation;

namespace DijitalEvrakTakip.Application.Features.AtlasDocumentNumberFeatures.Commands.ReserveAtlasDocumentNumbers;

public sealed class ReserveAtlasDocumentNumbersValidator : AbstractValidator<ReserveAtlasDocumentNumbersCommand>
{
    private const int MaxCount = 100;

    public ReserveAtlasDocumentNumbersValidator()
    {
        RuleFor(p => p.UserId)
            .NotEmpty().WithMessage("Kullanıcı bilgisi boş geçilemez")
            .Must(id => Guid.TryParse(id, out _)).WithMessage("Kullanıcı bilgisi geçersiz");

        RuleFor(p => p.Count)
            .InclusiveBetween(1, MaxCount).WithMessage($"Tek seferde 1 ile {MaxCount} arasında numara ayrılabilir");
    }
}
