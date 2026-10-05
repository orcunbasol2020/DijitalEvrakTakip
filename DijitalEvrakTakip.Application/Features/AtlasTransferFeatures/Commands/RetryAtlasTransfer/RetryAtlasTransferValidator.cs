using FluentValidation;

namespace DijitalEvrakTakip.Application.Features.AtlasTransferFeatures.Commands.RetryAtlasTransfer;

public sealed class RetryAtlasTransferValidator : AbstractValidator<RetryAtlasTransferCommand>
{
    public RetryAtlasTransferValidator()
    {
        RuleFor(p => p.DocumentId)
            .NotEmpty().WithMessage("Evrak seçilmedi");

        RuleFor(p => p.UserId)
            .NotEmpty().WithMessage("Kullanıcı bilgisi boş geçilemez");
    }
}
