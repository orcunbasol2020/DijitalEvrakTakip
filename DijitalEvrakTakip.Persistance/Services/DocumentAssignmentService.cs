using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace DijitalEvrakTakip.Persistance.Services;

public sealed class DocumentAssignmentService : IDocumentAssignmentService
{
    private readonly IDocumentAssignmentRepository _documentAssignmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DocumentAssignmentService(
        IDocumentAssignmentRepository documentAssignmentRepository,
        IUnitOfWork unitOfWork)
    {
        _documentAssignmentRepository = documentAssignmentRepository;
        _unitOfWork = unitOfWork;
    }

    // SaveChanges çağrısı yapmıyoruz, handler tek commit yapacak
    public void Create(DocumentAssignment documentAssignment)
    {
        documentAssignment.CreatedDate = DateTime.Now;
        _documentAssignmentRepository.Add(documentAssignment);
    }

    public void Update(DocumentAssignment assignment)
    {
        _documentAssignmentRepository.Update(assignment);
    }

    public async Task<DocumentAssignment?> GetActiveByDocumentIdAsync(
        Guid documentId,
        CancellationToken cancellationToken)
    {
        return await _documentAssignmentRepository
            .GetAll()
            .FirstOrDefaultAsync(
                x => x.DocumentId == documentId
                     && x.IsActive == true
                     && !x.IsDeleted,
                cancellationToken);
    }

    public async Task<IList<DocumentAssignmentDto>> GetByDocumentIdAsync(
        Guid documentId,
        CancellationToken cancellationToken)
    {
        return await _documentAssignmentRepository
            .GetAll()
            .Where(x => x.DocumentId == documentId && !x.IsDeleted)
            .Select(x => new DocumentAssignmentDto(
                x.Id,
                x.DocumentId,
                x.UserId,
                x.Lock,
                x.IsActive,
                x.CreatedDate
            ))
            .ToListAsync(cancellationToken);
    }

    public async Task<DocumentAssignment?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _documentAssignmentRepository
            .GetAll()
            .FirstOrDefaultAsync(
                x => x.Id == id && !x.IsDeleted,
                cancellationToken);
    }

    public async Task<IList<DocumentAssignment>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        return await _documentAssignmentRepository
            .GetAll()
            .Where(x => !x.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}