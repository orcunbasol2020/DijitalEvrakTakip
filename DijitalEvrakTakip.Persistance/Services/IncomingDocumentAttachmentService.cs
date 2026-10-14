using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Enums;
using DijitalEvrakTakip.Domain.Repositories;
using GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace DijitalEvrakTakip.Persistance.Services;

public sealed class IncomingDocumentAttachmentService : IIncomingDocumentAttachmentService
{
    private readonly IIncomingDocumentAttachmentRepository _attachmentRepository;
    private readonly IIncomingDocumentRepository _incomingDocumentRepository;
    private readonly IDocumentTransactionRepository _documentTransactionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public IncomingDocumentAttachmentService(
        IIncomingDocumentAttachmentRepository attachmentRepository,
        IIncomingDocumentRepository incomingDocumentRepository,
        IDocumentTransactionRepository documentTransactionRepository,
        IUnitOfWork unitOfWork)
    {
        _attachmentRepository = attachmentRepository;
        _incomingDocumentRepository = incomingDocumentRepository;
        _documentTransactionRepository = documentTransactionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task CreateAsync(
        IncomingDocumentAttachment attachment,
        string userId,
        CancellationToken cancellationToken)
    {
        var document = await _incomingDocumentRepository
            .GetByExpressionAsync(x => x.Id == attachment.IncomingDocumentId && !x.IsDeleted, cancellationToken);

        if (document is null)
            throw new Exception("Evrak bulunamadı.");

        var lastOrderNo = await _attachmentRepository
            .GetAll()
            .Where(x => x.IncomingDocumentId == attachment.IncomingDocumentId && !x.IsDeleted)
            .MaxAsync(x => (int?)x.OrderNo, cancellationToken);

        attachment.OrderNo = (lastOrderNo ?? 0) + 1;
        attachment.UploadedUserId = userId;

        await _attachmentRepository.AddAsync(attachment, cancellationToken);

        // Elektronik eki olan evrak "ek var" sayılır; fiziki ek açıklamasına dokunulmaz
        if (document.HasAttachment != true)
        {
            document.HasAttachment = true;
            _incomingDocumentRepository.Update(document);
        }

        await _documentTransactionRepository.AddAsync(new DocumentTransaction
        {
            DocumentId = attachment.IncomingDocumentId,
            TransactionType = (int)TransactionTypeEnum.EkYukleme,
            UserId = userId,
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(IncomingDocumentAttachment attachment, CancellationToken cancellationToken)
    {
        _attachmentRepository.Update(attachment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAsync(Guid id, string userId, CancellationToken cancellationToken)
    {
        var attachment = await _attachmentRepository
            .GetByExpressionAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

        if (attachment is null)
            throw new Exception("Ek bulunamadı.");

        attachment.IsDeleted = true;
        _attachmentRepository.Update(attachment);

        await _documentTransactionRepository.AddAsync(new DocumentTransaction
        {
            DocumentId = attachment.IncomingDocumentId,
            TransactionType = (int)TransactionTypeEnum.EkSilme,
            UserId = userId,
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IncomingDocumentAttachment?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _attachmentRepository
            .GetByExpressionAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<IList<IncomingDocumentAttachment>> GetByIncomingDocumentIdAsync(
        Guid incomingDocumentId,
        CancellationToken cancellationToken)
    {
        return await _attachmentRepository
            .GetAll()
            .Where(x => x.IncomingDocumentId == incomingDocumentId && !x.IsDeleted)
            .OrderBy(x => x.OrderNo)
            .ThenBy(x => x.CreatedDate)
            .ToListAsync(cancellationToken);
    }
}
