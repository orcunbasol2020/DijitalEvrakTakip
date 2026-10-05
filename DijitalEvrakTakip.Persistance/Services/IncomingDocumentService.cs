using DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Commands.CreateIncomingDocument;
using DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Commands.UpdateIncomingDocument;
using DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Queries.GetAllIncomingDocument;
using DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Queries.GetDocumentsByStatus;
using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Enums;
using DijitalEvrakTakip.Domain.Repositories;
using GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace DijitalEvrakTakip.Persistance.Services;

public sealed class IncomingDocumentService : IIncomingDocumentService
{
    private readonly IIncomingDocumentRepository _incomingDocumentRepository;
    private readonly IDocumentTransactionRepository _documentTransactionRepository;
    private readonly IScannedDocumentRepository _scannedDocumentRepository;
    private readonly IDocumentAllocationRepository _allocationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly IAtlasDocumentNumberPoolService _atlasDocumentNumberPoolService;
    private readonly IAtlasTransferService _atlasTransferService;

    public IncomingDocumentService(
        IIncomingDocumentRepository incomingDocumentRepository,
        IDocumentTransactionRepository documentTransactionRepository,
        IScannedDocumentRepository scannedDocumentRepository,
        IDocumentAllocationRepository allocationRepository,
        IUserRepository userRepository,
        IAtlasDocumentNumberPoolService atlasDocumentNumberPoolService,
        IAtlasTransferService atlasTransferService,
        IUnitOfWork unitOfWork)
    {
        _atlasDocumentNumberPoolService = atlasDocumentNumberPoolService;
        _atlasTransferService = atlasTransferService;
        _incomingDocumentRepository = incomingDocumentRepository;
        _documentTransactionRepository = documentTransactionRepository;
        _scannedDocumentRepository = scannedDocumentRepository;
        _allocationRepository = allocationRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task AttachUploadedFileAsync(
        Guid documentId,
        string savedFileName,
        string savedFullPath,
        string originalFileName,
        string userId,
        CancellationToken cancellationToken)
    {
        var entity = await _incomingDocumentRepository
            .GetByExpressionAsync(x => x.Id == documentId && !x.IsDeleted, cancellationToken);

        if (entity is null)
            throw new Exception("Evrak bulunamadı.");

        entity.DocumentName = savedFileName;
        entity.ElectronicCopy = true;
        entity.Status = (int)DocumentStatusEnum.Update;
        entity.UpdateDate = DateTime.UtcNow;
        _incomingDocumentRepository.Update(entity);

        // Taranan evraklarla aynı kayıt yapısını koru: DocumentNumber dolu olduğu için
        // eşleştirme bekleyenler listesinde görünmez.
        var scanned = await _scannedDocumentRepository
            .GetByExpressionAsync(x => x.DocumentNumber == entity.QrCode && !x.IsDeleted, cancellationToken);

        if (scanned is null)
        {
            scanned = new ScannedDocument
            {
                DocumentNumber = entity.QrCode,
                FileName = savedFileName,
                OriginalPath = originalFileName,
                NewPath = savedFullPath,
                IsDeleted = false,
                CreatedDate = DateTime.UtcNow
            };
            await _scannedDocumentRepository.AddAsync(scanned, cancellationToken);
        }
        else
        {
            scanned.FileName = savedFileName;
            scanned.OriginalPath = originalFileName;
            scanned.NewPath = savedFullPath;
            scanned.UpdateDate = DateTime.UtcNow;
            _scannedDocumentRepository.Update(scanned);
        }

        var transaction = new DocumentTransaction
        {
            DocumentId = entity.Id,
            TransactionType = (int)TransactionTypeEnum.FileUpload,
            UserId = userId,
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };
        await _documentTransactionRepository.AddAsync(transaction, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task AttachUploadedFileByNumberAsync(
        string documentNumber,
        string savedFileName,
        string savedFullPath,
        string originalFileName,
        string userId,
        CancellationToken cancellationToken)
    {
        var entity = await _incomingDocumentRepository
            .GetByExpressionAsync(x => x.QrCode == documentNumber && !x.IsDeleted, cancellationToken);

        // Taranmış belge eşleştirmesindeki kural: numara başka bir dosyaya bağlıysa üzerine yazılmaz
        var numberUsedByScan = await _scannedDocumentRepository
            .AnyAsync(x => !x.IsDeleted && x.DocumentNumber == documentNumber, cancellationToken);

        if (numberUsedByScan || !string.IsNullOrWhiteSpace(entity?.DocumentName))
            throw new Exception($"{documentNumber} numaralı evraka daha önce bir dosya bağlanmış.");

        bool hasActiveAllocation;

        if (entity is not null)
        {
            entity.DocumentName = savedFileName;
            entity.ElectronicCopy = true;
            entity.Status = (int)DocumentStatusEnum.Update;
            entity.UpdateDate = DateTime.UtcNow;
            _incomingDocumentRepository.Update(entity);

            hasActiveAllocation = await _allocationRepository
                .AnyAsync(x => x.IncomingDocumentId == entity.Id && x.IsActive && !x.IsDeleted, cancellationToken);
        }
        else
        {
            entity = new IncomingDocument
            {
                QrCode = documentNumber,
                DocumentName = savedFileName,
                ElectronicCopy = true,
                OcrStatus = (int?)OcrStatusEnum.Wait,
                Status = (int)DocumentStatusEnum.Update,
                Release = false,
                UserId = userId,
                CreatedUserId = userId,
                IsDeleted = false,
                CreatedDate = DateTime.UtcNow
            };
            await _incomingDocumentRepository.AddAsync(entity, cancellationToken);

            await _atlasDocumentNumberPoolService.MarkUsedForIncomingDocumentAsync(documentNumber, entity.Id, cancellationToken);

            hasActiveAllocation = false;
        }

        // Eşleştirme bekleyenler listesinde görünmemesi için DocumentNumber dolu kayıt açılır
        await _scannedDocumentRepository.AddAsync(new ScannedDocument
        {
            DocumentNumber = documentNumber,
            FileName = savedFileName,
            OriginalPath = originalFileName,
            NewPath = savedFullPath,
            IsDeleted = false,
            CreatedDate = DateTime.UtcNow
        }, cancellationToken);

        await _documentTransactionRepository.AddAsync(new DocumentTransaction
        {
            DocumentId = entity.Id,
            TransactionType = (int)TransactionTypeEnum.FileUpload,
            UserId = userId,
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        }, cancellationToken);

        // Evrağın aktif zimmeti yoksa yükleyen kullanıcıya zimmetlenir (DocumentAllocations/Create ile aynı kayıtlar)
        if (!hasActiveAllocation && Guid.TryParse(userId, out var allocationUserId))
        {
            await _allocationRepository.AddAsync(new DocumentAllocation
            {
                IncomingDocumentId = entity.Id,
                UserId = allocationUserId,
                UserType = (int)AllocationUserTypeEnum.Internal,
                CreatedUserId = allocationUserId,
                Status = (int)AllocationStatusEnum.OnKayit,
                Source = (int)AllocationSourceEnum.EvrakTakip,
                IsActive = true
            }, cancellationToken);

            await _documentTransactionRepository.AddAsync(new DocumentTransaction
            {
                DocumentId = entity.Id,
                TransactionType = (int)TransactionTypeEnum.Zimmet,
                UserId = userId,
                CreatedUserId = userId,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            }, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task CreateAsync(CreateIncomingDocumentCommand request, CancellationToken cancellationToken)
    {
        IncomingDocument incomingDocument = new()
        {
            Id = Guid.NewGuid(),
            OrginalNo = request.OrginalNo,
            QrCode = request.QrCode,
            SecurityDegree = request.SecurityDegree,
            UrgencyDegree = request.UrgencyDegree,
            DocumentTypeId = request.DocumentTypeId,
            LanguageId = request.LanguageId,
            Subject = request.Subject,
            Content_Ocr = request.Content_Ocr,
            ExternalInstitutionId = request.ExternalInstitutionId,
            DepartmentId = request.DepartmentId,
            Status = request.Status,
            ElectronicCopy = request.ElectronicCopy,
            Release = request.Release,
            ActionRequired = request.ActionRequired,
            PageCount = request.PageCount,
            DocumentDate = request.DocumentDate,
            ReleaseDate = request.ReleaseDate,
            OcrStatus = request.OcrStatus,
            // Yayın durumunu yalnızca Atlas aktarım kuyruğu değiştirir
            SubmissionStatus = (int)PublishStatusEnum.Yayinlanmadi,
            UserId = request.UserId,
            CreatedUserId = request.CreatedUserId ?? request.UserId,
            DocumentName = request.DocumentName,
            Notes = request.Notes,
            HasAttachment = request.HasAttachment,
            // Ek yok denmişse açıklama tutulmaz
            AttachmentDescription = request.HasAttachment == false
                ? null
                : NormalizeAttachmentDescription(request.AttachmentDescription),
            IsDeleted = false,
            CreatedDate = DateTime.UtcNow
        };

        await _incomingDocumentRepository.AddAsync(incomingDocument, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(UpdateIncomingDocumentCommand request, CancellationToken cancellationToken)
    {
        var entity = await _incomingDocumentRepository
            .GetByExpressionAsync(x => x.Id == request.Id, cancellationToken);

        if (entity is null)
            throw new Exception("Kayıt bulunamadı.");

        if (request.OrginalNo != null) entity.OrginalNo = request.OrginalNo;
        if (request.QrCode != null) entity.QrCode = request.QrCode;
        if (request.SecurityDegree.HasValue) entity.SecurityDegree = request.SecurityDegree;
        if (request.UrgencyDegree.HasValue) entity.UrgencyDegree = request.UrgencyDegree;
        if (request.DocumentTypeId.HasValue) entity.DocumentTypeId = request.DocumentTypeId;
        if (request.LanguageId.HasValue) entity.LanguageId = request.LanguageId;
        if (request.Subject != null) entity.Subject = request.Subject;
        if (request.ExternalInstitutionId.HasValue) entity.ExternalInstitutionId = request.ExternalInstitutionId;
        if (request.DepartmentId.HasValue) entity.DepartmentId = request.DepartmentId;
        // Yayınla akış durumu değildir: Status korunur, evrak yalnızca aktarım sırasına alınır
        var isPublish = request.Status == (int)DocumentStatusEnum.Publish;
        if (!isPublish && request.Status.HasValue) entity.Status = request.Status;
        if (request.ElectronicCopy.HasValue) entity.ElectronicCopy = request.ElectronicCopy;
        if (request.Release.HasValue) entity.Release = request.Release;
        if (request.ActionRequired.HasValue) entity.ActionRequired = request.ActionRequired;
        if (request.PageCount.HasValue) entity.PageCount = request.PageCount;
        if (request.DocumentDate.HasValue) entity.DocumentDate = request.DocumentDate;
        if (request.ReleaseDate.HasValue) entity.ReleaseDate = request.ReleaseDate;
        if (request.DocumentName != null) entity.DocumentName = request.DocumentName;
        if (request.Notes != null) entity.Notes = request.Notes;
        if (request.HasAttachment.HasValue) entity.HasAttachment = request.HasAttachment;
        // null gönderilirse mevcut açıklama korunur, boş metin açıklamayı temizler
        if (request.AttachmentDescription != null) entity.AttachmentDescription = NormalizeAttachmentDescription(request.AttachmentDescription);
        if (entity.HasAttachment == false) entity.AttachmentDescription = null;

        entity.UpdateDate = DateTime.UtcNow;

        // Güncel bilgilerle kontrol edilir; eksik bilgi varsa hiçbir değişiklik kaydedilmez
        if (isPublish) await _atlasTransferService.EnqueueAsync(entity, cancellationToken);

        _incomingDocumentRepository.Update(entity);

        // Yeni transaction ekle
        var transaction = new DocumentTransaction
        {
            DocumentId = entity.Id,
            TransactionType = isPublish
                ? (int)TransactionTypeEnum.Yayinla
                : (int)TransactionTypeEnum.Update,
            UserId = request.UserId,
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };
        await _documentTransactionRepository.AddAsync(transaction, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static string? NormalizeAttachmentDescription(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    public async Task<IncomingDocument?> GetByQrCodeAsync(string qrCode, CancellationToken cancellationToken)
    {
        return await _incomingDocumentRepository.GetByExpressionAsync(
            x => x.QrCode == qrCode,
            cancellationToken
        );
    }

    public async Task<IncomingDocument?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _incomingDocumentRepository.GetByExpressionAsync(
            x => x.Id == id,
            cancellationToken
        );
    }

    public async Task<IList<IncomingDocument>> GetAllAsync(GetAllIncomingDocumentQuery request, CancellationToken cancellationToken)
    {
        var query = _incomingDocumentRepository.GetAll()
            .Where(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.Status) && request.Status != "all")
        {
            query = request.Status switch
            {
                "completed" => query.Where(x => x.OcrStatus == 1),
                "pending" => query.Where(x => x.OcrStatus == 0),
                "error" => query.Where(x => x.OcrStatus == 2),
                _ => query
            };
        }

        // Oluşturan kullanıcı yalnızca CreatedUserId sütunundan okunur (UserId'ye düşülmez)
        if (!string.IsNullOrEmpty(request.CreatedUserId))
            query = query.Where(x => x.CreatedUserId == request.CreatedUserId);

        if (request.DepartmentId.HasValue)
            query = query.Where(x => x.DepartmentId == request.DepartmentId.Value);

        return await query.ToListAsync(cancellationToken);
    }
    public async Task<IList<IncomingDocument>> GetAllByDirectionAsync(string? documentDirection, CancellationToken cancellationToken)
    {
        var query = _incomingDocumentRepository.GetAll()
            .Where(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(documentDirection) && documentDirection != "all")
        {
            query = documentDirection switch
            {
                "incoming" => query.Where(x => x.DocumentDirection == (int)DocumentDirectionEnum.Incoming),
                "outgoing" => query.Where(x => x.DocumentDirection == (int)DocumentDirectionEnum.Outgoing),
                _ => query
            };
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IList<IncomingDocument>> GetAllByStatusAsync(GetDocumentsByStatusQuery request, CancellationToken cancellationToken)
    {
        var query = _incomingDocumentRepository.GetAll()
            .Where(x => !x.IsDeleted && x.Status == request.Status);

        if (request.DepartmentId.HasValue)
            query = query.Where(x => x.DepartmentId == request.DepartmentId.Value);

        // Oluşturan kullanıcı yalnızca CreatedUserId sütunundan okunur (UserId'ye düşülmez)
        if (!string.IsNullOrEmpty(request.CreatedUserId))
            query = query.Where(x => x.CreatedUserId == request.CreatedUserId);

        return await query
            .OrderByDescending(x => x.CreatedDate)
            .ToListAsync(cancellationToken);
    }
    /// <summary>
    /// Assignment atandığında IncomingDocument tablosundaki CurrentAssignmentUserId alanını set eder.
    /// SaveChanges handler tarafında yapılacak.
    /// </summary>
    public async Task SetCurrentAssignmentAsync(Guid documentId, Guid? userId)
    {
        // Evrakı getir
        var entity = _incomingDocumentRepository
            .GetAll()
            .FirstOrDefault(x => x.Id == documentId);

        if (entity is null)
            throw new Exception("Evrak bulunamadı.");

        entity.CurrentAssignmentUserId = userId;

        if (userId.HasValue)
        {
            // Kullanıcı bilgilerini repository’den async al
            var user = await _userRepository
                .GetAll()
                .FirstOrDefaultAsync(u => u.Id == userId.Value);

            // Ad + Soyad alanını birleştir
            entity.CurrentAssignmentUser = user != null
                ? $"{user.Name} {user.Surname}"
                : "Bilinmeyen Kullanıcı";
        }
        else
        {
            entity.CurrentAssignmentUser = "";
        }

        entity.UpdateDate = DateTime.UtcNow;

        _incomingDocumentRepository.Update(entity);
    }

    public async Task<int> GetPendingCountByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _incomingDocumentRepository
            .GetAll()
            .Where(x =>
                !x.IsDeleted &&
                x.CurrentAssignmentUserId == userId &&
                (
                    x.Status == (int)DocumentStatusEnum.OnKayit ||
                    x.Status == (int)DocumentStatusEnum.Update ||
                    x.Status == (int)DocumentStatusEnum.Match
                ))
            .CountAsync(cancellationToken);
    }

    public async Task<IncomingDocumentTodayStatsDto> GetTodayStatsAsync(CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;
        var yesterday = today.AddDays(-1);

        var todayCount = await _incomingDocumentRepository
            .GetAll()
            .Where(x => !x.IsDeleted && x.CreatedDate.Date == today)
            .CountAsync(cancellationToken);

        var yesterdayCount = await _incomingDocumentRepository
            .GetAll()
            .Where(x => !x.IsDeleted && x.CreatedDate.Date == yesterday)
            .CountAsync(cancellationToken);

        double changePercent = 0;

        if (yesterdayCount > 0)
        {
            changePercent = ((double)(todayCount - yesterdayCount) / yesterdayCount) * 100;
        }

        return new IncomingDocumentTodayStatsDto
        {
            TodayCount = todayCount,
            ChangePercent = changePercent
        };
    }

    public async Task<IncomingDocumentLast30DaysStatsDto> GetLast30DaysStatsAsync(CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;

        var startLast30 = today.AddDays(-30);
        var startPrev30 = today.AddDays(-60);
        var endPrev30 = today.AddDays(-30);

        var last30DaysCount = await _incomingDocumentRepository
            .GetAll()
            .Where(x =>
                !x.IsDeleted &&
                x.CreatedDate >= startLast30 &&
                x.CreatedDate < today)
            .CountAsync(cancellationToken);

        var prev30DaysCount = await _incomingDocumentRepository
            .GetAll()
            .Where(x =>
                !x.IsDeleted &&
                x.CreatedDate >= startPrev30 &&
                x.CreatedDate < endPrev30)
            .CountAsync(cancellationToken);

        double changePercent = 0;

        if (prev30DaysCount > 0)
        {
            changePercent = ((double)(last30DaysCount - prev30DaysCount) / prev30DaysCount) * 100;
        }

        return new IncomingDocumentLast30DaysStatsDto
        {
            Last30DaysCount = last30DaysCount,
            ChangePercent = changePercent
        };
    }

    public async Task<IncomingDocumentPendingScanStatsDto> GetPendingScanStatsAsync(CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;
        var yesterday = today.AddDays(-1);

        var pendingCount = await _incomingDocumentRepository
            .GetAll()
            .Where(x =>
                !x.IsDeleted &&
                string.IsNullOrEmpty(x.DocumentName))
            .CountAsync(cancellationToken);

        var yesterdayCount = await _incomingDocumentRepository
            .GetAll()
            .Where(x =>
                !x.IsDeleted &&
                !string.IsNullOrEmpty(x.DocumentName) &&
                x.CreatedDate.Date == yesterday)
            .CountAsync(cancellationToken);

        double changePercent = 0;

        if (yesterdayCount > 0)
        {
            changePercent = ((double)(pendingCount - yesterdayCount) / yesterdayCount) * 100;
        }

        return new IncomingDocumentPendingScanStatsDto
        {
            PendingScanCount = pendingCount,
            ChangePercent = changePercent
        };
    }

    public async Task<IncomingDocumentOcrQueueStatsDto> GetOcrQueueStatsAsync(CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;
        var yesterday = today.AddDays(-1);

        // Bugünkü OCR kuyruğundaki toplam kayıt (OCR bekleyenler)
        var ocrQueueCount = await _incomingDocumentRepository
            .GetAll()
            .Where(x =>
                !x.IsDeleted &&
                x.OcrStatus == 0) // 0 = pending OCR
            .CountAsync(cancellationToken);

        // Bir gün öncesi OCR tamamlanmış kayıtlar
        var yesterdayCount = await _incomingDocumentRepository
            .GetAll()
            .Where(x =>
                !x.IsDeleted &&
                x.OcrStatus != 0 &&
                x.CreatedDate.Date == yesterday)
            .CountAsync(cancellationToken);

        double changePercent = 0;
        if (yesterdayCount > 0)
        {
            changePercent = ((double)(ocrQueueCount - yesterdayCount) / yesterdayCount) * 100;
        }

        return new IncomingDocumentOcrQueueStatsDto
        {
            OcrQueueCount = ocrQueueCount,
            ChangePercent = changePercent
        };
    }
}