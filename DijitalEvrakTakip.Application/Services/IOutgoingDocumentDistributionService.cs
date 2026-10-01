using DijitalEvrakTakip.Domain.Entities;

namespace DijitalEvrakTakip.Application.Services;

public interface IOutgoingDocumentDistributionService
{
    Task CreateRangeAsync(IList<OutgoingDocumentDistribution> distributions, CancellationToken cancellationToken);

    Task UpdateAsync(OutgoingDocumentDistribution distribution, CancellationToken cancellationToken);

    Task<OutgoingDocumentDistribution?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IList<OutgoingDocumentDistribution>> GetByOutgoingDocumentIdAsync(Guid outgoingDocumentId, CancellationToken cancellationToken);

    Task<IList<OutgoingDocumentDistribution>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken);

    Task<IList<OutgoingDocumentDistribution>> GetByShipmentIdAsync(Guid shipmentId, CancellationToken cancellationToken);

    Task UpdateRangeAsync(IList<OutgoingDocumentDistribution> distributions, CancellationToken cancellationToken);

    Task RemoveAsync(Guid id, CancellationToken cancellationToken);
}
