using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;

namespace DijitalEvrakTakip.Application.Services;

public interface IDocumentAllocationService
{
    Task CreateAsync(
        DocumentAllocation allocation,
        CancellationToken cancellationToken);

    Task<IList<DocumentAllocationDto>> GetByDocumentIdAsync(
        Guid incomingDocumentId,
        CancellationToken cancellationToken);

    Task<DocumentAllocation?> GetActiveByDocumentIdAsync(
        Guid incomingDocumentId,
        CancellationToken cancellationToken);

    Task<DocumentAllocationDto?> GetActiveDtoByDocumentIdAsync(
        Guid incomingDocumentId,
        CancellationToken cancellationToken);

    Task<DocumentAllocation?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task UpdateAsync(
        DocumentAllocation allocation,
        CancellationToken cancellationToken);

    Task<IList<UserActiveAllocationDto>> GetActiveByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task<UserAllocationTransferCountDto> GetTransferCountByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken);
}