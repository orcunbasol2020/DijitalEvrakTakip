using DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Commands.CreateIncomingDocument;
using DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Commands.UpdateIncomingDocument;
using DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Queries.GetAllIncomingDocument;
using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Enums;
using DijitalEvrakTakip.Domain.Repositories;
using DijitalEvrakTakip.Persistance.Repositories;
using GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace DijitalEvrakTakip.Persistance.Services;

public sealed class IncomingDocumentService : IIncomingDocumentService
{
    private readonly IIncomingDocumentRepository _incomingDocumentRepository;
    private readonly IDocumentTransactionRepository _documentTransactionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public IncomingDocumentService(
        IIncomingDocumentRepository incomingDocumentRepository,
        IDocumentTransactionRepository documentTransactionRepository,
        IUnitOfWork unitOfWork)
    {
        _incomingDocumentRepository = incomingDocumentRepository;
        _documentTransactionRepository = documentTransactionRepository;
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

        entity.OrginalNo = request.OrginalNo;
        entity.QrCode = request.QrCode;
        entity.SecurityDegree = request.SecurityDegree;
        entity.DocumentTypeId = request.DocumentTypeId;
        entity.LanguageId = request.LanguageId;
        entity.Subject = request.Subject;
        entity.Content_Ocr = request.Content_Ocr;
        entity.ExternalInstitutionId = request.ExternalInstitutionId;
        entity.DepartmentId = request.DepartmentId;
        entity.Status = request.Status;
        entity.ElectronicCopy = request.ElectronicCopy;
        entity.Release = request.Release;
        entity.PageCount = request.PageCount;
        entity.DocumentDate = request.DocumentDate;
        entity.ReleaseDate = request.ReleaseDate;
        entity.OcrStatus = request.OcrStatus;
        entity.SubmissionStatus = request.SubmissionStatus;
        entity.DocumentName = request.DocumentName;
        entity.Notes = request.Notes;
        entity.UpdateDate = DateTime.UtcNow;

        _incomingDocumentRepository.Update(entity); // incomingDocument update

        // Yeni transaction ekle
        var transaction = new DocumentTransaction
        {
            DocumentId = entity.Id,
            TransactionType = (int)TransactionTypeEnum.Update, // Güncelleme
            UserId = request.UserId,
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };
        await _documentTransactionRepository.AddAsync(transaction, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IncomingDocument> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _incomingDocumentRepository.GetByExpressionAsync(
            x => x.Id == id,
            cancellationToken
        );
    }


    public async Task<IncomingDocument?> GetByQrCodeAsync(string qrCode, CancellationToken cancellationToken)
    {
        return await _incomingDocumentRepository.GetByExpressionAsync(
    x => x.QrCode == qrCode,
    cancellationToken
);
    }

    public async Task<IList<IncomingDocument>> GetAllAsync(GetAllIncomingDocumentQuery request, CancellationToken cancellationToken)
    {
        var query = _incomingDocumentRepository.GetAll()
                       .Where(x => !x.IsDeleted); // opsiyonel, silinmişleri filtrele

        if (!string.IsNullOrWhiteSpace(request.Status) && request.Status != "all")
        {
            query = request.Status switch
            {
                "completed" => query.Where(x => x.Status == 1),
                "pending" => query.Where(x => x.Status == 0),
                "error" => query.Where(x => x.Status == 2),
                _ => query
            };
        }

        return await query.ToListAsync(cancellationToken);
    }


}
