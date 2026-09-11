using DijitalEvrakTakip.Domain.Entities;

namespace DijitalEvrakTakip.Application.Services;

public interface IEnvelopeDocumentService
{
    Task CreateAsync(EnvelopeDocument envelopeDocument, CancellationToken cancellationToken);
    Task<IList<EnvelopeDocument>> GetAllAsync(CancellationToken cancellationToken);
    Task<IList<EnvelopeDocument>> GetByEnvelopeIdAsync(Guid envelopeId, CancellationToken cancellationToken);
    Task<IList<EnvelopeDocument>> GetByDocumentIdAsync(Guid documentId, CancellationToken cancellationToken);
    Task RemoveAsync(Guid id, CancellationToken cancellationToken);
}