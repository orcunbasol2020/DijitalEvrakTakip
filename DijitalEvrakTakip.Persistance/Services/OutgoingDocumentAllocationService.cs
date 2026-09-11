using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Enums;
using DijitalEvrakTakip.Domain.Repositories;
using GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace DijitalEvrakTakip.Persistance.Services;

public sealed class OutgoingDocumentAllocationService : IOutgoingDocumentAllocationService
{
    private readonly IOutgoingDocumentAllocationRepository _allocationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IExternalUserRepository _externalUserRepository;
    private readonly IOutgoingDocumentRepository _outgoingDocumentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public OutgoingDocumentAllocationService(
        IOutgoingDocumentAllocationRepository allocationRepository,
        IUserRepository userRepository,
        IExternalUserRepository externalUserRepository,
        IOutgoingDocumentRepository outgoingDocumentRepository,
        IUnitOfWork unitOfWork)
    {
        _allocationRepository = allocationRepository;
        _userRepository = userRepository;
        _externalUserRepository = externalUserRepository;
        _outgoingDocumentRepository = outgoingDocumentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task CreateAsync(
        OutgoingDocumentAllocation allocation,
        CancellationToken cancellationToken)
    {
        await _allocationRepository.AddAsync(allocation, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<OutgoingDocumentAllocation?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _allocationRepository
            .GetAll()
            .FirstOrDefaultAsync(
                x => x.Id == id && !x.IsDeleted,
                cancellationToken);
    }

    public async Task UpdateAsync(
        OutgoingDocumentAllocation allocation,
        CancellationToken cancellationToken)
    {
        _allocationRepository.Update(allocation);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<OutgoingDocumentAllocation?> GetActiveByDocumentIdAsync(
        Guid outgoingDocumentId,
        CancellationToken cancellationToken)
    {
        return await _allocationRepository
            .GetAll()
            .Where(x => x.OutgoingDocumentId == outgoingDocumentId
                        && x.IsActive
                        && !x.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<OutgoingDocumentAllocationDto?> GetActiveDtoByDocumentIdAsync(
        Guid outgoingDocumentId,
        CancellationToken cancellationToken)
    {
        var allocation = await GetActiveByDocumentIdAsync(outgoingDocumentId, cancellationToken);

        if (allocation is null)
            return null;

        var fullNames = await GetFullNamesAsync(new[] { allocation }, cancellationToken);

        return ToDto(allocation, fullNames);
    }

    public async Task<IList<OutgoingDocumentAllocationDto>> GetByDocumentIdAsync(
        Guid outgoingDocumentId,
        CancellationToken cancellationToken)
    {
        var allocations = await _allocationRepository
            .GetAll()
            .Where(x => x.OutgoingDocumentId == outgoingDocumentId
                        && !x.IsDeleted)
            .OrderBy(x => x.CreatedDate)
            .ToListAsync(cancellationToken);

        var fullNames = await GetFullNamesAsync(allocations, cancellationToken);

        return allocations
            .Select(x => ToDto(x, fullNames))
            .ToList();
    }

    private static OutgoingDocumentAllocationDto ToDto(
        OutgoingDocumentAllocation allocation,
        IReadOnlyDictionary<Guid, string> fullNames)
    {
        return new OutgoingDocumentAllocationDto
        {
            Id = allocation.Id,
            OutgoingDocumentId = allocation.OutgoingDocumentId,
            UserId = allocation.UserId.ToString(),
            UserType = allocation.UserType,
            FullName = fullNames.TryGetValue(allocation.UserId, out var name) ? name : string.Empty,
            CreatedFullName = allocation.CreatedUserId.HasValue
                && fullNames.TryGetValue(allocation.CreatedUserId.Value, out var createdName)
                    ? createdName
                    : null,
            Status = allocation.Status,
            Source = allocation.Source,
            IsActive = allocation.IsActive,
            IsDeleted = allocation.IsDeleted,
            CreatedDate = allocation.CreatedDate,
            UpdateDate = allocation.UpdateDate
        };
    }

    private async Task<Dictionary<Guid, string>> GetFullNamesAsync(
        IEnumerable<OutgoingDocumentAllocation> allocations,
        CancellationToken cancellationToken)
    {
        var internalIds = allocations
            .Where(x => x.UserType == (int)AllocationUserTypeEnum.Internal)
            .Select(x => x.UserId)
            .Concat(allocations
                .Where(x => x.CreatedUserId.HasValue)
                .Select(x => x.CreatedUserId!.Value))
            .Distinct()
            .ToList();

        var externalIds = allocations
            .Where(x => x.UserType == (int)AllocationUserTypeEnum.External)
            .Select(x => x.UserId)
            .Distinct()
            .ToList();

        var result = new Dictionary<Guid, string>();

        if (internalIds.Count > 0)
        {
            var users = await _userRepository
                .GetAll()
                .Where(x => internalIds.Contains(x.Id))
                .Select(x => new { x.Id, x.Name, x.Surname })
                .ToListAsync(cancellationToken);

            foreach (var user in users)
                result[user.Id] = $"{user.Name} {user.Surname}";
        }

        if (externalIds.Count > 0)
        {
            var externalUsers = await _externalUserRepository
                .GetAll()
                .Where(x => externalIds.Contains(x.Id))
                .Select(x => new { x.Id, x.Name, x.Surname })
                .ToListAsync(cancellationToken);

            foreach (var externalUser in externalUsers)
                result[externalUser.Id] = $"{externalUser.Name} {externalUser.Surname}";
        }

        return result;
    }

    public async Task<IList<UserActiveOutgoingAllocationDto>> GetActiveByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var allocations = await _allocationRepository
            .GetAll()
            .Where(x => x.UserId == userId
                        && x.IsActive
                        && !x.IsDeleted)
            .OrderByDescending(x => x.CreatedDate)
            .ToListAsync(cancellationToken);

        var documentIds = allocations
            .Select(x => x.OutgoingDocumentId)
            .Distinct()
            .ToList();

        var documentsById = await _outgoingDocumentRepository
            .GetAll()
            .Where(x => documentIds.Contains(x.Id))
            .Select(x => new
            {
                x.Id,
                x.QrCode,
                x.OriginalDocumentNumber,
                x.Subject,
                x.DocumentDate
            })
            .ToDictionaryAsync(x => x.Id, cancellationToken);

        return allocations
            .Select(x =>
            {
                documentsById.TryGetValue(x.OutgoingDocumentId, out var document);

                return new UserActiveOutgoingAllocationDto
                {
                    AllocationId = x.Id,
                    OutgoingDocumentId = x.OutgoingDocumentId,
                    QrCode = document?.QrCode,
                    OriginalDocumentNumber = document?.OriginalDocumentNumber,
                    Subject = document?.Subject,
                    DocumentDate = document?.DocumentDate,
                    Status = x.Status,
                    Source = x.Source,
                    AllocatedDate = x.CreatedDate
                };
            })
            .ToList();
    }

    public async Task<UserAllocationTransferCountDto> GetTransferCountByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var count = await _allocationRepository
            .GetAll()
            .Where(x => x.CreatedUserId == userId && !x.IsDeleted)
            .CountAsync(cancellationToken);

        return new UserAllocationTransferCountDto
        {
            UserId = userId,
            TransferCount = count
        };
    }
}
