using DijitalEvrakTakip.Domain.Entities;

namespace DijitalEvrakTakip.Application.Services;

public interface IDocumentTransactionService
{
    Task CreateAsync(DocumentTransaction transaction, CancellationToken cancellationToken);
    Task<IList<DocumentTransaction>> GetByDocumentIdAsync(Guid documentId, CancellationToken cancellationToken);
}
