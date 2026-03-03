using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace DijitalEvrakTakip.Persistance.Services;

public sealed class DocumentTransactionService : IDocumentTransactionService
{
    private readonly IDocumentTransactionRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DocumentTransactionService(
        IDocumentTransactionRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IList<DocumentTransaction>> GetByDocumentIdAsync(
        Guid documentId,
        CancellationToken cancellationToken)
    {
        return await _repository
            .GetAll()
            .Where(d => d.DocumentId == documentId)
            .ToListAsync(cancellationToken);
    }

    public async Task CreateAsync(
        DocumentTransaction transaction,
        CancellationToken cancellationToken)
    {
        await _repository.AddAsync(transaction, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
