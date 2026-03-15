using DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Commands.CreateIncomingDocument;
using DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Commands.UpdateIncomingDocument;
using DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Queries.GetAllIncomingDocument;
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
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;

    public IncomingDocumentService(
        IIncomingDocumentRepository incomingDocumentRepository,
        IDocumentTransactionRepository documentTransactionRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _incomingDocumentRepository = incomingDocumentRepository;
        _documentTransactionRepository = documentTransactionRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task CreateAsync(CreateIncomingDocumentCommand request, CancellationToken cancellationToken)
    {
        IncomingDocument incomingDocument = new()
        {
            Id = Guid.NewGuid(),
            OrginalNo = request.OrginalNo,
            QrCode = request.QrCode,
            SecurityDegree = request.SecurityDegree,
            DocumentTypeId = request.DocumentTypeId,
            LanguageId = request.LanguageId,
            Subject = request.Subject,
            Content_Ocr = request.Content_Ocr,
            ExternalInstitutionId = request.ExternalInstitutionId,
            DepartmentId = request.DepartmentId,
            Status = request.Status,
            ElectronicCopy = request.ElectronicCopy,
            Release = request.Release,
            PageCount = request.PageCount,
            DocumentDate = request.DocumentDate,
            ReleaseDate = request.ReleaseDate,
            OcrStatus = request.OcrStatus,
            SubmissionStatus = request.SubmissionStatus,
            UserId = request.UserId,
            DocumentName = request.DocumentName,
            Notes = request.Notes,
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
        if (request.DocumentTypeId.HasValue) entity.DocumentTypeId = request.DocumentTypeId;
        if (request.LanguageId.HasValue) entity.LanguageId = request.LanguageId;
        if (request.Subject != null) entity.Subject = request.Subject;
        if (request.ExternalInstitutionId.HasValue) entity.ExternalInstitutionId = request.ExternalInstitutionId;
        if (request.DepartmentId.HasValue) entity.DepartmentId = request.DepartmentId;
        if (request.Status.HasValue) entity.Status = request.Status;
        if (request.ElectronicCopy.HasValue) entity.ElectronicCopy = request.ElectronicCopy;
        if (request.Release.HasValue) entity.Release = request.Release;
        if (request.PageCount.HasValue) entity.PageCount = request.PageCount;
        if (request.DocumentDate.HasValue) entity.DocumentDate = request.DocumentDate;
        if (request.ReleaseDate.HasValue) entity.ReleaseDate = request.ReleaseDate;
        if (request.SubmissionStatus.HasValue) entity.SubmissionStatus = request.SubmissionStatus;
        if (request.DocumentName != null) entity.DocumentName = request.DocumentName;
        if (request.Notes != null) entity.Notes = request.Notes;

        entity.UpdateDate = DateTime.UtcNow;

        _incomingDocumentRepository.Update(entity);

        // Yeni transaction ekle
        var transaction = new DocumentTransaction
        {
            DocumentId = entity.Id,
            TransactionType = request.Status.HasValue && request.Status.Value == (int)DocumentStatusEnum.Publish
                ? (int)TransactionTypeEnum.Yayinla
                : (int)TransactionTypeEnum.Update,
            UserId = request.UserId,
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };
        await _documentTransactionRepository.AddAsync(transaction, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

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

        return await query.ToListAsync(cancellationToken);
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
}