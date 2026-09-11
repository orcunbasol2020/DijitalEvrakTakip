using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace DijitalEvrakTakip.Persistance.Services;

public sealed class EnvelopeDocumentService : IEnvelopeDocumentService
{
    private readonly IEnvelopeDocumentRepository _envelopeDocumentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EnvelopeDocumentService(
        IEnvelopeDocumentRepository envelopeDocumentRepository,
        IUnitOfWork unitOfWork)
    {
        _envelopeDocumentRepository = envelopeDocumentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task CreateAsync(
        EnvelopeDocument envelopeDocument,
        CancellationToken cancellationToken)
    {
        await _envelopeDocumentRepository.AddAsync(envelopeDocument, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IList<EnvelopeDocument>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        return await _envelopeDocumentRepository
            .GetAll()
            .Where(x => !x.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<IList<EnvelopeDocument>> GetByEnvelopeIdAsync(
      Guid envelopeId,
      CancellationToken cancellationToken)
    {
        return await _envelopeDocumentRepository
            .GetAll()
            .Where(x => !x.IsDeleted && x.EnvelopeId == envelopeId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IList<EnvelopeDocument>> GetByDocumentIdAsync(
      Guid documentId,
      CancellationToken cancellationToken)
    {
        return await _envelopeDocumentRepository
            .GetAll()
            .Where(x => !x.IsDeleted && x.DocumentId == documentId)
            .ToListAsync(cancellationToken);
    }

    public async Task RemoveAsync(Guid id, CancellationToken cancellationToken)
    {
        var envelopeDocument = await _envelopeDocumentRepository
            .GetAll()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (envelopeDocument is null)
            throw new Exception("Kayıt bulunamadı");

        envelopeDocument.IsDeleted = true;

        _envelopeDocumentRepository.Update(envelopeDocument);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}