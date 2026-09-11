using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Enums;
using DijitalEvrakTakip.Domain.Repositories;
using GenericRepository;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace DijitalEvrakTakip.Persistance.Services;

public sealed class DocumentAllocationService : IDocumentAllocationService
{
    private readonly IDocumentAllocationRepository _allocationRepository;
    private readonly IDocumentTransactionRepository _transactionRepository;
    private readonly IUserRepository _userRepository;
    private readonly IExternalUserRepository _externalUserRepository;
    private readonly IIncomingDocumentRepository _incomingDocumentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DocumentAllocationService(
        IDocumentAllocationRepository allocationRepository,
        IDocumentTransactionRepository transactionRepository,
        IUserRepository userRepository,
        IExternalUserRepository externalUserRepository,
        IIncomingDocumentRepository incomingDocumentRepository,
        IUnitOfWork unitOfWork)
    {
        _allocationRepository = allocationRepository;
        _transactionRepository = transactionRepository;
        _userRepository = userRepository;
        _externalUserRepository = externalUserRepository;
        _incomingDocumentRepository = incomingDocumentRepository;
        _unitOfWork = unitOfWork;
    }

    //public async Task CreateAsync(
    //    DocumentAllocation allocation,
    //    CancellationToken cancellationToken)
    //{
    //    await _allocationRepository.AddAsync(allocation, cancellationToken);
    //    await _unitOfWork.SaveChangesAsync(cancellationToken);
    //}

    public async Task CreateAsync(
        DocumentAllocation allocation,
        CancellationToken cancellationToken)
    {
        // Allocation insert
        await _allocationRepository.AddAsync(allocation, cancellationToken);

        int tansactionStatus = (int)TransactionTypeEnum.Zimmet;
        if (allocation.Status == 3)
            tansactionStatus = (int)TransactionTypeEnum.TeslimZimmet;

        // Transaction insert
        var transaction = new DocumentTransaction
        {
            DocumentId = allocation.IncomingDocumentId,
            TransactionType = tansactionStatus,
            UserId = allocation.UserId.ToString(),
            CreatedUserId = allocation.CreatedUserId.ToString(),
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };

        await _transactionRepository.AddAsync(transaction, cancellationToken);

        // Tek commit
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<DocumentAllocation?> GetByIdAsync(
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
        DocumentAllocation allocation,
        CancellationToken cancellationToken)
    {
        _allocationRepository.Update(allocation);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<DocumentAllocation?> GetActiveByDocumentIdAsync(
        Guid incomingDocumentId,
        CancellationToken cancellationToken)
    {
        return await _allocationRepository
            .GetAll()
            .Where(x => x.IncomingDocumentId == incomingDocumentId
                        && x.IsActive
                        && !x.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<DocumentAllocationDto?> GetActiveDtoByDocumentIdAsync(
        Guid incomingDocumentId,
        CancellationToken cancellationToken)
    {
        var allocation = await GetActiveByDocumentIdAsync(incomingDocumentId, cancellationToken);

        if (allocation is null)
            return null;

        var fullNames = await GetFullNamesAsync(new[] { allocation }, cancellationToken);

        return ToDto(allocation, fullNames);
    }

    public async Task<IList<DocumentAllocationDto>> GetByDocumentIdAsync(
        Guid incomingDocumentId,
        CancellationToken cancellationToken)
    {
        var allocations = await _allocationRepository
            .GetAll()
            .Where(x => x.IncomingDocumentId == incomingDocumentId
                        && !x.IsDeleted)
            .OrderBy(x => x.CreatedDate)
            .ToListAsync(cancellationToken);

        var fullNames = await GetFullNamesAsync(allocations, cancellationToken);

        return allocations
            .Select(x => ToDto(x, fullNames))
            .ToList();
    }

    private static DocumentAllocationDto ToDto(
        DocumentAllocation allocation,
        IReadOnlyDictionary<Guid, string> fullNames)
    {
        return new DocumentAllocationDto
        {
            Id = allocation.Id,
            IncomingDocumentId = allocation.IncomingDocumentId,
            UserId = allocation.UserId.ToString(),
            UserType = allocation.UserType,
            FullName = fullNames.TryGetValue(allocation.UserId, out var name) ? name : string.Empty,
            Status = allocation.Status,
            Source = allocation.Source,
            IsActive = allocation.IsActive,
            IsDeleted = allocation.IsDeleted,
            CreatedDate = allocation.CreatedDate,
            UpdateDate = allocation.UpdateDate
        };
    }

    private async Task<Dictionary<Guid, string>> GetFullNamesAsync(
        IEnumerable<DocumentAllocation> allocations,
        CancellationToken cancellationToken)
    {
        var internalIds = allocations
            .Where(x => x.UserType == (int)AllocationUserTypeEnum.Internal)
            .Select(x => x.UserId)
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

    public async Task<IList<UserActiveAllocationDto>> GetActiveByUserIdAsync(
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
            .Select(x => x.IncomingDocumentId)
            .Distinct()
            .ToList();

        var documentsById = await _incomingDocumentRepository
            .GetAll()
            .Where(x => documentIds.Contains(x.Id))
            .Select(x => new
            {
                x.Id,
                x.OrginalNo,
                x.QrCode,
                x.DocumentName,
                x.Subject,
                x.DocumentDate
            })
            .ToDictionaryAsync(x => x.Id, cancellationToken);

        return allocations
            .Select(x =>
            {
                documentsById.TryGetValue(x.IncomingDocumentId, out var document);

                return new UserActiveAllocationDto
                {
                    AllocationId = x.Id,
                    IncomingDocumentId = x.IncomingDocumentId,
                    OrginalNo = document?.OrginalNo,
                    QrCode = document?.QrCode,
                    DocumentName = document?.DocumentName,
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