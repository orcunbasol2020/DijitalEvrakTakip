using System.Text.Json;
using DijitalEvrakTakip.Application.Features.AtlasEbysFeatures.Commands.ReceiveZimmetChange;
using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Enums;
using DijitalEvrakTakip.Domain.Repositories;
using GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace DijitalEvrakTakip.Persistance.Services;

/// <summary>
/// Atlas EBYS entegrasyonu tamamlanana kadar zimmet değişikliği bildirimlerini
/// karşılayan dummy (stub) servis. Gelen her bildirimi ham haliyle saklar,
/// mümkünse yerel evrağa eşleştirip zimmet bilgisini günceller.
/// </summary>
public sealed class AtlasEbysService : IAtlasEbysService
{
    private readonly IAtlasZimmetChangeRepository _atlasZimmetChangeRepository;
    private readonly IIncomingDocumentRepository _incomingDocumentRepository;
    private readonly IDocumentTransactionRepository _documentTransactionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AtlasEbysService(
        IAtlasZimmetChangeRepository atlasZimmetChangeRepository,
        IIncomingDocumentRepository incomingDocumentRepository,
        IDocumentTransactionRepository documentTransactionRepository,
        IUnitOfWork unitOfWork)
    {
        _atlasZimmetChangeRepository = atlasZimmetChangeRepository;
        _incomingDocumentRepository = incomingDocumentRepository;
        _documentTransactionRepository = documentTransactionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<MessageResponse> ReceiveZimmetChangeAsync(
        ReceiveAtlasZimmetChangeCommand request,
        CancellationToken cancellationToken)
    {
        var alreadyReceived = await _atlasZimmetChangeRepository
            .GetAll()
            .FirstOrDefaultAsync(x => x.AtlasZimmetId == request.AtlasZimmetId, cancellationToken);

        if (alreadyReceived != null)
            return new MessageResponse("Bu zimmet bildirimi daha önce alınmış");

        var incomingDocument = await _incomingDocumentRepository
            .GetAll()
            .FirstOrDefaultAsync(
                x => !x.IsDeleted &&
                     ((x.OrginalNo != null && x.OrginalNo == request.AtlasDocumentNo) ||
                      (request.QrCode != null && x.QrCode == request.QrCode)),
                cancellationToken);

        var zimmetChange = new AtlasZimmetChange
        {
            AtlasZimmetId = request.AtlasZimmetId,
            AtlasDocumentNo = request.AtlasDocumentNo,
            QrCode = request.QrCode,
            FromUserSicilNo = request.FromUserSicilNo,
            FromUserName = request.FromUserName,
            FromUnitName = request.FromUnitName,
            ToUserSicilNo = request.ToUserSicilNo,
            ToUserName = request.ToUserName,
            ToUnitName = request.ToUnitName,
            ZimmetTuru = request.ZimmetTuru,
            ZimmetTarihi = request.ZimmetTarihi,
            Description = request.Description,
            RawPayload = JsonSerializer.Serialize(request),
            IncomingDocumentId = incomingDocument?.Id
        };

        if (incomingDocument == null)
        {
            zimmetChange.Status = (int)AtlasZimmetChangeStatusEnum.Unmatched;
            zimmetChange.ErrorMessage = "Bildirimle eşleşen evrak bulunamadı";

            await _atlasZimmetChangeRepository.AddAsync(zimmetChange, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new MessageResponse("Zimmet bildirimi alındı ancak eşleşen evrak bulunamadı", zimmetChange.Id);
        }

        incomingDocument.CurrentAssignmentUser = request.ToUserName;
        incomingDocument.CurrentAssignmentUserId = null;
        _incomingDocumentRepository.Update(incomingDocument);

        var transaction = new DocumentTransaction
        {
            DocumentId = incomingDocument.Id,
            TransactionType = (int)TransactionTypeEnum.Zimmet,
            UserId = request.ToUserSicilNo ?? request.ToUserName,
            CreatedUserId = "AtlasEbys",
            IsActive = true
        };

        zimmetChange.Status = (int)AtlasZimmetChangeStatusEnum.Processed;
        zimmetChange.ProcessedDate = DateTime.UtcNow;

        await _atlasZimmetChangeRepository.AddAsync(zimmetChange, cancellationToken);
        await _documentTransactionRepository.AddAsync(transaction, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new MessageResponse("Zimmet değişikliği başarıyla işlendi", zimmetChange.Id);
    }

    public async Task<IList<AtlasZimmetChange>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _atlasZimmetChangeRepository
            .GetAll()
            .Where(x => !x.IsDeleted)
            .ToListAsync(cancellationToken);
    }
}
