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
    private readonly IUnitOfWork _unitOfWork;

    public DocumentAllocationService(
        IDocumentAllocationRepository allocationRepository,
        IDocumentTransactionRepository transactionRepository,
        IUnitOfWork unitOfWork)
    {
        _allocationRepository = allocationRepository;
        _transactionRepository = transactionRepository;
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
            .FirstOrDefaultAsync(
                x => x.IncomingDocumentId == incomingDocumentId
                     && x.IsActive
                     && !x.IsDeleted,
                cancellationToken);
    }

    public async Task<IList<DocumentAllocationDto>> GetByDocumentIdAsync(
        Guid incomingDocumentId,
        CancellationToken cancellationToken)
    {
        return await _allocationRepository
            .GetAll()
            .Where(x => x.IncomingDocumentId == incomingDocumentId
                        && !x.IsDeleted)
            .OrderBy(x => x.CreatedDate)
            .Select(x => new DocumentAllocationDto
            {
                Id = x.Id,
                IncomingDocumentId = x.IncomingDocumentId,
                UserId = x.UserId.ToString(),
                FullName = x.User.Name + " " + x.User.Surname,
                Status = x.Status,
                IsActive = x.IsActive,
                IsDeleted = x.IsDeleted,
                CreatedDate = x.CreatedDate,
                UpdateDate = x.UpdateDate
            })
            .ToListAsync(cancellationToken);
    }
}