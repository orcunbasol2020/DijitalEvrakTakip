using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;

namespace DijitalEvrakTakip.Application.Services;

public interface IOutgoingDocumentAllocationService
{
    Task CreateAsync(
        OutgoingDocumentAllocation allocation,
        CancellationToken cancellationToken);

    Task<IList<OutgoingDocumentAllocationDto>> GetByDocumentIdAsync(
        Guid outgoingDocumentId,
        CancellationToken cancellationToken);

    Task<OutgoingDocumentAllocation?> GetActiveByDocumentIdAsync(
        Guid outgoingDocumentId,
        CancellationToken cancellationToken);

    Task<OutgoingDocumentAllocationDto?> GetActiveDtoByDocumentIdAsync(
        Guid outgoingDocumentId,
        CancellationToken cancellationToken);

    Task<OutgoingDocumentAllocation?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task UpdateAsync(
        OutgoingDocumentAllocation allocation,
        CancellationToken cancellationToken);

    Task<IList<UserActiveOutgoingAllocationDto>> GetActiveByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task<UserAllocationTransferCountDto> GetTransferCountByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken);
}
