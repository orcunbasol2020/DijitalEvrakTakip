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
    private readonly IDocumentAllocationRepository _allocationRepository;
    private readonly IDocumentTransactionRepository _transactionRepository;
    private readonly IAtlasDocumentNumberPoolService _atlasDocumentNumberPoolService;
    private readonly IUnitOfWork _unitOfWork;

    public ScannedDocumentService(
        IScannedDocumentRepository scannedDocumentRepository,
        IIncomingDocumentRepository incomingDocumentRepository,
        IDocumentAllocationRepository allocationRepository,
        IDocumentTransactionRepository transactionRepository,
        IAtlasDocumentNumberPoolService atlasDocumentNumberPoolService,
        IUnitOfWork unitOfWork)
    {
        _atlasDocumentNumberPoolService = atlasDocumentNumberPoolService;
        _scannedDocumentRepository = scannedDocumentRepository;
        _incomingDocumentRepository = incomingDocumentRepository;
        _allocationRepository = allocationRepository;
        _transactionRepository = transactionRepository;
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

        if (request.StartDate.HasValue)
        {
            query = query.Where(x => x.CreatedDate >= request.StartDate.Value);
        }

        if (request.EndDate.HasValue)
        {
            // Saat verilmediyse bitiş gününün tamamı dahil edilir
            var end = request.EndDate.Value.TimeOfDay == TimeSpan.Zero
                ? request.EndDate.Value.Date.AddDays(1)
                : request.EndDate.Value;

            query = query.Where(x => x.CreatedDate < end);
        }

        return await query
            .OrderByDescending(x => x.CreatedDate)
            .ToListAsync(cancellationToken);
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

        var incomingDocument = await _incomingDocumentRepository
            .GetByExpressionAsync(x => x.QrCode == request.DocumentNumber, cancellationToken);

        // Daha önce kullanılmış numara reddedilir: başka bir taranmış belgeye verilmişse ya da
        // bu numaralı gelen evrağa zaten bir dosya bağlıysa, eşleştirme mevcut dosyanın üzerine yazardı.
        var numberUsedByScan = await _scannedDocumentRepository
            .AnyAsync(x => x.Id != request.Id && !x.IsDeleted && x.DocumentNumber == request.DocumentNumber, cancellationToken);

        if (numberUsedByScan || !string.IsNullOrWhiteSpace(incomingDocument?.DocumentName))
            throw new Exception($"{request.DocumentNumber} numarası daha önce başka bir taranmış evrakla eşleştirilmiş.");

        // DocumentNumber ve UpdateDate güncelle
        entity.DocumentNumber = request.DocumentNumber;
        entity.UpdateDate = DateTime.UtcNow;
        _scannedDocumentRepository.Update(entity);

        // IncomingDocument ile eşleştirme
        bool hasActiveAllocation;

        if (incomingDocument != null)
        {
            // Eşleşen kayıt varsa, DocumentName güncelle
            incomingDocument.DocumentName = entity.FileName;
            _incomingDocumentRepository.Update(incomingDocument);

            hasActiveAllocation = await _allocationRepository
                .AnyAsync(x => x.IncomingDocumentId == incomingDocument.Id && x.IsActive && !x.IsDeleted, cancellationToken);
        }
        else
        {
            // Eşleşen kayıt yoksa, yeni kayıt oluştur
            incomingDocument = new IncomingDocument
            {
                QrCode = entity.DocumentNumber ?? "",
                DocumentName = entity.FileName,
                OcrStatus = (int?)OcrStatusEnum.Wait,
                Status = (int?)DocumentStatusEnum.Match,
                Release = false,
                UserId = request.UserId,
            };
            await _incomingDocumentRepository.AddAsync(incomingDocument, cancellationToken);

            await _atlasDocumentNumberPoolService.MarkUsedForIncomingDocumentAsync(incomingDocument.QrCode, incomingDocument.Id, cancellationToken);

            hasActiveAllocation = false;
        }

        // Evrağın aktif zimmeti yoksa eşleştirmeyi yapan kullanıcıya zimmetlenir
        if (!hasActiveAllocation && Guid.TryParse(request.UserId, out var userId))
            await AllocateToUserAsync(incomingDocument.Id, userId, cancellationToken);

        // Tüm değişiklikleri kaydet
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    // DocumentAllocations/Create ile aynı kayıtlar (zimmet + Zimmet işlem kaydı) oluşturulur;
    // SaveChanges çağıran metotta tek seferde yapılır
    private async Task AllocateToUserAsync(
        Guid incomingDocumentId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        await _allocationRepository.AddAsync(new DocumentAllocation
        {
            IncomingDocumentId = incomingDocumentId,
            UserId = userId,
            UserType = (int)AllocationUserTypeEnum.Internal,
            CreatedUserId = userId,
            Status = (int)AllocationStatusEnum.OnKayit,
            Source = (int)AllocationSourceEnum.EvrakTakip,
            IsActive = true
        }, cancellationToken);

        await _transactionRepository.AddAsync(new DocumentTransaction
        {
            DocumentId = incomingDocumentId,
            TransactionType = (int)TransactionTypeEnum.Zimmet,
            UserId = userId.ToString(),
            CreatedUserId = userId.ToString(),
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        }, cancellationToken);
    }
}