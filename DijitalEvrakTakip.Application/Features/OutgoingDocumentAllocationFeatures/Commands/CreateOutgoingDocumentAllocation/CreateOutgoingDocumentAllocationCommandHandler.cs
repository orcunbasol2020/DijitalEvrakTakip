using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Enums;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.OutgoingDocumentAllocationFeatures.Commands.CreateOutgoingDocumentAllocation;

public sealed class CreateOutgoingDocumentAllocationCommandHandler
    : IRequestHandler<CreateOutgoingDocumentAllocationCommand, MessageResponse>
{
    private readonly IOutgoingDocumentAllocationService _allocationService;
    private readonly IEnvelopeDocumentService _envelopeDocumentService;
    private readonly IEnvelopeService _envelopeService;
    private readonly IOutgoingDocumentService _outgoingDocumentService;

    public CreateOutgoingDocumentAllocationCommandHandler(
        IOutgoingDocumentAllocationService allocationService,
        IEnvelopeDocumentService envelopeDocumentService,
        IEnvelopeService envelopeService,
        IOutgoingDocumentService outgoingDocumentService)
    {
        _allocationService = allocationService;
        _envelopeDocumentService = envelopeDocumentService;
        _envelopeService = envelopeService;
        _outgoingDocumentService = outgoingDocumentService;
    }

    public async Task<MessageResponse> Handle(
        CreateOutgoingDocumentAllocationCommand request,
        CancellationToken cancellationToken)
    {
        var outgoingDocument = await _outgoingDocumentService.GetByIdAsync(
            request.OutgoingDocumentId,
            cancellationToken);

        if (outgoingDocument is null)
            return new MessageResponse(
                "Geçersiz OutgoingDocumentId: bu Id'ye sahip bir evrak bulunamadı. " +
                "Zarfa evrak eklerken dönen 'DocumentId' alanı gönderilmelidir, 'Id' alanı değil.");

        var activeAllocation =
            await _allocationService.GetActiveByDocumentIdAsync(
                request.OutgoingDocumentId,
                cancellationToken);

        // Aktif zimmet var mı?
        if (activeAllocation != null)
        {
            // 1.a Aynı kişideyse engelle
            if (activeAllocation.UserId == Guid.Parse(request.UserId))
                return new MessageResponse("Bu evrak zaten bu kullanıcıya zimmetli");

            // 1.b Başka kişideyse pasife çek
            activeAllocation.IsActive = false;

            await _allocationService.UpdateAsync(activeAllocation, cancellationToken);
        }

        // Yeni aktif zimmet oluştur
        var allocation = new OutgoingDocumentAllocation
        {
            OutgoingDocumentId = request.OutgoingDocumentId,
            UserId = Guid.Parse(request.UserId),
            UserType = request.UserType,
            CreatedUserId = Guid.Parse(request.CreatedUserId),
            Status = Convert.ToInt32(request.Status),
            Source = (int)AllocationSourceEnum.EvrakTakip,
            IsActive = true
        };

        await _allocationService.CreateAsync(allocation, cancellationToken);

        await MarkContainingEnvelopesAsDeliveredAsync(request.OutgoingDocumentId, cancellationToken);

        return new MessageResponse("Evrak başarıyla zimmetlendi");
    }

    private async Task MarkContainingEnvelopesAsDeliveredAsync(
        Guid outgoingDocumentId,
        CancellationToken cancellationToken)
    {
        var envelopeDocuments = await _envelopeDocumentService
            .GetByDocumentIdAsync(outgoingDocumentId, cancellationToken);

        var envelopeIds = envelopeDocuments
            .Select(x => x.EnvelopeId)
            .Distinct();

        foreach (var envelopeId in envelopeIds)
        {
            var envelope = await _envelopeService.GetByIdAsync(envelopeId, cancellationToken);

            if (envelope is null || envelope.Status == (int)EnvelopeStatusEnum.Delivered)
                continue;

            envelope.Status = (int)EnvelopeStatusEnum.Delivered;

            await _envelopeService.UpdateAsync(envelope, cancellationToken);
        }
    }
}
