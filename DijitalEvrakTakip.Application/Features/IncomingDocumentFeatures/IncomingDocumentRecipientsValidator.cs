using FluentValidation;

namespace DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures;

/// <summary>Gelen evrak oluşturma ve güncellemede gönderilen "Nereye" listesinin ortak kuralları.</summary>
public sealed class IncomingDocumentRecipientsValidator : AbstractValidator<IList<IncomingDocumentRecipient>>
{
    public IncomingDocumentRecipientsValidator()
    {
        RuleForEach(p => p)
            .Must(r => r.DepartmentId != Guid.Empty)
            .WithMessage("Her alıcı için birim seçilmelidir!");

        RuleForEach(p => p)
            .Must(r => r.Notes is null || r.Notes.Length <= 500)
            .WithMessage("Alıcı notu en fazla 500 karakter olabilir");

        RuleFor(p => p)
            .Must(r => r.Select(x => x.DepartmentId).Distinct().Count() == r.Count)
            .WithMessage("Aynı birim birden fazla kez alıcı olarak eklenemez!");
    }
}
