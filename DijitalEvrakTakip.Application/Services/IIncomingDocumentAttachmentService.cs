using DijitalEvrakTakip.Domain.Entities;

namespace DijitalEvrakTakip.Application.Services;

public interface IIncomingDocumentAttachmentService
{
    /// <summary>
    /// Eki evrakın ek listesinin sonuna ekler, evrakı "ek var" olarak işaretler ve Ek Yükleme işlemini yazar.
    /// </summary>
    Task CreateAsync(IncomingDocumentAttachment attachment, string userId, CancellationToken cancellationToken);

    Task UpdateAsync(IncomingDocumentAttachment attachment, CancellationToken cancellationToken);

    /// <summary>Eki kaldırır (soft delete, dosya diskte kalır) ve Ek Silme işlemini yazar.</summary>
    Task RemoveAsync(Guid id, string userId, CancellationToken cancellationToken);

    Task<IncomingDocumentAttachment?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>Evrakın silinmemiş eklerini EYP sırasıyla (OrderNo, yüklenme zamanı) getirir.</summary>
    Task<IList<IncomingDocumentAttachment>> GetByIncomingDocumentIdAsync(Guid incomingDocumentId, CancellationToken cancellationToken);
}
