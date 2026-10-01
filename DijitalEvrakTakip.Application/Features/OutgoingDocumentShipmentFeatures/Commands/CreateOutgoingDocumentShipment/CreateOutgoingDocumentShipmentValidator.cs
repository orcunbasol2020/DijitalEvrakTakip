using DijitalEvrakTakip.Domain.Enums;
using FluentValidation;

namespace DijitalEvrakTakip.Application.Features.OutgoingDocumentShipmentFeatures.Commands.CreateOutgoingDocumentShipment;

public sealed class CreateOutgoingDocumentShipmentValidator
    : AbstractValidator<CreateOutgoingDocumentShipmentCommand>
{
    public CreateOutgoingDocumentShipmentValidator()
    {
        RuleFor(p => p.DistributionIds)
            .NotEmpty().WithMessage("En az bir dağıtım kaydı seçilmelidir!");

        RuleFor(p => p.CargoCompany)
            .Must(x => Enum.IsDefined(typeof(CargoCompanyEnum), x))
            .WithMessage("Geçersiz kargo firması!");

        RuleFor(p => p.TrackingNumber)
            .NotEmpty().WithMessage("Kargo takip numarası boş olamaz!")
            .MaximumLength(100).WithMessage("Kargo takip numarası en fazla 100 karakter olabilir!");

        RuleFor(p => p.SentUserId)
            .NotEmpty().WithMessage("Kargoya veren kullanıcı boş olamaz!");

        RuleFor(p => p.RecipientName)
            .MaximumLength(200).WithMessage("Alıcı adı en fazla 200 karakter olabilir!");

        RuleFor(p => p.Notes)
            .MaximumLength(500).WithMessage("Not en fazla 500 karakter olabilir!");
    }
}
