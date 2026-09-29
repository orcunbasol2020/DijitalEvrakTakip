using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;

namespace DijitalEvrakTakip.Application.Services;

public interface IDocumentAllocationService
{
    // previousAllocation verilirse yeni zimmetle aynı commit'te pasife çekilir
    Task CreateAsync(
        DocumentAllocation allocation,
        DocumentAllocation? previousAllocation,
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

    Task ReceiveAsync(
        DocumentAllocation allocation,
        CancellationToken cancellationToken);

    Task<IList<UserActiveAllocationDto>> GetActiveByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task<UserAllocationTransferCountDto> GetTransferCountByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken);
}