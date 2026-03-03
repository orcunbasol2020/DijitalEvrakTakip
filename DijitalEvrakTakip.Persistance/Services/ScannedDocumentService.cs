using DijitalEvrakTakip.Application.Features.ScannedDocumentFeatures.Commands.UpdateScannedDocument;
using DijitalEvrakTakip.Application.Features.ScannedDocumentFeatures.Queries.GetAllScannedDocument;
using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace DijitalEvrakTakip.Persistance.Services;

public sealed class ScannedDocumentService : IScannedDocumentService
{
    private readonly IScannedDocumentRepository _scannedDocumentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ScannedDocumentService(
        IScannedDocumentRepository scannedDocumentRepository,
        IUnitOfWork unitOfWork)
    {
        _scannedDocumentRepository = scannedDocumentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task CreateAsync(
        ScannedDocument scannedDocument,
        CancellationToken cancellationToken)
    {
        await _scannedDocumentRepository.AddAsync(scannedDocument, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IList<ScannedDocument>> GetAllAsync(
        GetAllScannedDocumentQuery request,
        CancellationToken cancellationToken)
    {
        var query = _scannedDocumentRepository
            .GetAll()
            .Where(x => !x.IsDeleted && x.DocumentNumber == null);

        if (!string.IsNullOrWhiteSpace(request.FileName))
        {
            query = query.Where(x => x.FileName!.Contains(request.FileName));
        }

        return await query.ToListAsync(cancellationToken);
    }

    // Yeni güncelleme metodunu ekliyoruz.
    public async Task UpdateDocumentNumberAsync(
        UpdateScannedDocumentCommand request, // UpdateCommand parametresi alıyoruz
        CancellationToken cancellationToken)
    {
        // Kayıt kontrolü (scanned document)
        var entity = await _scannedDocumentRepository
            .GetByExpressionAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);

        if (entity is null)
            throw new Exception("Belge bulunamadı.");

        // Güncellenen alan (DocumentNumber) null kontrolü yapılmaz çünkü zaten gelen request'te mevcut.
        entity.DocumentNumber = request.DocumentNumber;
        entity.UpdateDate = DateTime.UtcNow;

        _scannedDocumentRepository.Update(entity); // Veritabanı güncellemesi

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}