using DijitalEvrakTakip.Application.Features.OutgoingDocumentFeatures.Commands.CreateOutgoingDocument;
using DijitalEvrakTakip.Application.Features.OutgoingDocumentFeatures.Commands.UpdateOutgoingDocument;
using DijitalEvrakTakip.Application.Features.OutgoingDocumentFeatures.Queries.GetAllOutgoingDocument;
using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Enums;
using DijitalEvrakTakip.Domain.Repositories;
using GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace DijitalEvrakTakip.Persistance.Services;

public sealed class OutgoingDocumentService : IOutgoingDocumentService
{
    private readonly IOutgoingDocumentRepository _outgoingDocumentRepository;
    private readonly IOutgoingDocumentTransactionRepository _outgoingDocumentTransactionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public OutgoingDocumentService(
        IOutgoingDocumentRepository outgoingDocumentRepository,
        IOutgoingDocumentTransactionRepository outgoingDocumentTransactionRepository,
        IUnitOfWork unitOfWork)
    {
        _outgoingDocumentRepository = outgoingDocumentRepository;
        _outgoingDocumentTransactionRepository = outgoingDocumentTransactionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> CreateAsync(CreateOutgoingDocumentCommand request, CancellationToken cancellationToken)
    {
        OutgoingDocument outgoingDocument = new()
        {
            QrCode = request.QrCode,
            OriginalDocumentNumber = request.OriginalDocumentNumber,
            SecurityDegree = request.SecurityDegree,
            Type = request.Type,
            LanguageId = request.LanguageId,
            Subject = request.Subject,
            Content_Ocr = request.Content_Ocr,
            Status = request.Status,
            DepartmentId = request.DepartmentId,
            ExternalInstitutonId = request.ExternalInstitutonId,
            ElectronicCopy = request.ElectronicCopy,
            EbysTransfer = request.EbysTransfer,
            PageCount = request.PageCount,
            Notes = request.Notes,
            DocumentDate = request.DocumentDate,
            CreatedUserId = request.CreatedUserId,
            IsDeleted = false
        };

        await _outgoingDocumentRepository.AddAsync(outgoingDocument, cancellationToken);

        var creationTransaction = new OutgoingDocumentTransaction
        {
            OutgoingDocumentId = outgoingDocument.Id,
            Type = request.Status ?? (int)OutgoingTransactionTypeEnum.Draft,
            UserId = request.CreatedUserId
        };

        await _outgoingDocumentTransactionRepository.AddAsync(creationTransaction, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return outgoingDocument.Id;
    }

    public async Task UpdateAsync(UpdateOutgoingDocumentCommand request, CancellationToken cancellationToken)
    {
        var entity = await _outgoingDocumentRepository
            .GetByExpressionAsync(x => x.Id == request.Id, cancellationToken);

        if (entity is null)
            throw new Exception("Kayıt bulunamadı.");

        if (request.QrCode != null) entity.QrCode = request.QrCode;
        if (request.OriginalDocumentNumber != null) entity.OriginalDocumentNumber = request.OriginalDocumentNumber;
        if (request.SecurityDegree != null) entity.SecurityDegree = request.SecurityDegree;
        if (request.Type.HasValue) entity.Type = request.Type;
        if (request.LanguageId.HasValue) entity.LanguageId = request.LanguageId;
        if (request.Subject != null) entity.Subject = request.Subject;
        if (request.Content_Ocr != null) entity.Content_Ocr = request.Content_Ocr;
        if (request.Status.HasValue) entity.Status = request.Status;
        if (request.DepartmentId.HasValue) entity.DepartmentId = request.DepartmentId;
        if (request.ExternalInstitutonId.HasValue) entity.ExternalInstitutonId = request.ExternalInstitutonId;
        if (request.ElectronicCopy.HasValue) entity.ElectronicCopy = request.ElectronicCopy;
        if (request.EbysTransfer.HasValue) entity.EbysTransfer = request.EbysTransfer;
        if (request.PageCount.HasValue) entity.PageCount = request.PageCount;
        if (request.Notes != null) entity.Notes = request.Notes;
        if (request.DocumentDate.HasValue) entity.DocumentDate = request.DocumentDate;
        if (request.CreatedUserId != null) entity.CreatedUserId = request.CreatedUserId;

        entity.UpdateDate = DateTime.UtcNow;

        _outgoingDocumentRepository.Update(entity);

        var transaction = new OutgoingDocumentTransaction
        {
            OutgoingDocumentId = entity.Id,
            Type = request.Status ?? (int)OutgoingTransactionTypeEnum.Update,
            CargoPostNumber = request.CargoPostNumber,
            UserId = request.CreatedUserId
        };

        await _outgoingDocumentTransactionRepository.AddAsync(transaction, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IList<OutgoingDocument>> GetAllAsync(GetAllOutgoingDocumentQuery request, CancellationToken cancellationToken)
    {
        var query = _outgoingDocumentRepository.GetAll()
            .Where(x => !x.IsDeleted);

        if (request.Status.HasValue)
            query = query.Where(x => x.Status == request.Status.Value);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<OutgoingDocument?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _outgoingDocumentRepository.GetByExpressionAsync(
            x => x.Id == id, cancellationToken);
    }

    public async Task<OutgoingDocument?> GetByQrCodeAsync(string qrCode, CancellationToken cancellationToken)
    {
        return await _outgoingDocumentRepository.GetByExpressionAsync(
            x => x.QrCode == qrCode, cancellationToken);
    }
}
