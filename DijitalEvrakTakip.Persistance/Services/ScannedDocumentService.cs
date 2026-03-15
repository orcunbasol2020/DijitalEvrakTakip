using DijitalEvrakTakip.Application.Features.ScannedDocumentFeatures.Commands.UpdateScannedDocument;
using DijitalEvrakTakip.Application.Features.ScannedDocumentFeatures.Queries.GetAllScannedDocument;
using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Enums;
using DijitalEvrakTakip.Domain.Repositories;
using GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace DijitalEvrakTakip.Persistance.Services;

public sealed class ScannedDocumentService : IScannedDocumentService
{
    private readonly IScannedDocumentRepository _scannedDocumentRepository;
    private readonly IIncomingDocumentRepository _incomingDocumentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ScannedDocumentService(
        IScannedDocumentRepository scannedDocumentRepository,
        IIncomingDocumentRepository incomingDocumentRepository,
        IUnitOfWork unitOfWork)
    {
        _scannedDocumentRepository = scannedDocumentRepository;
        _incomingDocumentRepository = incomingDocumentRepository;
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

    public async Task UpdateDocumentNumberAsync(
          UpdateScannedDocumentCommand request,
          CancellationToken cancellationToken)
    {
        // ScannedDocument kaydını bul
        var entity = await _scannedDocumentRepository
            .GetByExpressionAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);

        if (entity is null)
            throw new Exception("Belge bulunamadı.");

        // DocumentNumber ve UpdateDate güncelle
        entity.DocumentNumber = request.DocumentNumber;
        entity.UpdateDate = DateTime.UtcNow;
        _scannedDocumentRepository.Update(entity);

        // IncomingDocument ile eşleştirme
        var incomingDocument = await _incomingDocumentRepository
            .GetByExpressionAsync(x => x.QrCode == entity.DocumentNumber, cancellationToken);

        if (incomingDocument != null)
        {
            // Eşleşen kayıt varsa, DocumentName güncelle
            incomingDocument.DocumentName = entity.FileName;
            _incomingDocumentRepository.Update(incomingDocument);
        }
        else
        {
            // Eşleşen kayıt yoksa, yeni kayıt oluştur
            var newIncoming = new IncomingDocument
            {
                QrCode = entity.DocumentNumber ?? "",
                DocumentName = entity.FileName,
                OcrStatus = (int?)OcrStatusEnum.Wait,
                Status = (int?)DocumentStatusEnum.Match,
                Release = false,
                UserId = request.UserId,
            };
            await _incomingDocumentRepository.AddAsync(newIncoming, cancellationToken);
        }

        // Tüm değişiklikleri kaydet
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}