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
    private readonly IOutgoingDocumentService _outgoingDocumentService;

    public CreateOutgoingDocumentAllocationCommandHandler(
        IOutgoingDocumentAllocationService allocationService,
        IOutgoingDocumentService outgoingDocumentService)
    {
        _allocationService = allocationService;
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

        if (!int.TryParse(request.Status, out var status) ||
            !Enum.IsDefined(typeof(AllocationStatusEnum), status))
            return new MessageResponse(
                "Geçersiz Status: 1 (Ön Kayıt), 2 (Devir), 3 (Teslim), 4 (Arşiv) veya 5 (Teslim Alındı) olmalıdır.");

        var activeAllocation =
            await _allocationService.GetActiveByDocumentIdAsync(
                request.OutgoingDocumentId,
                cancellationToken);

        // Aktif zimmet aynı kişideyse engelle; başka kişideyse CreateAsync pasife çeker
        if (activeAllocation != null && activeAllocation.UserId == Guid.Parse(request.UserId))
            return new MessageResponse("Bu evrak zaten bu kullanıcıya zimmetli");

        // Yeni aktif zimmet oluştur
        var allocation = new OutgoingDocumentAllocation
        {
            OutgoingDocumentId = request.OutgoingDocumentId,
            UserId = Guid.Parse(request.UserId),
            UserType = request.UserType,
            CreatedUserId = Guid.Parse(request.CreatedUserId),
            Status = status,
            Source = (int)AllocationSourceEnum.EvrakTakip,
            IsActive = true
        };

        await _allocationService.CreateAsync(allocation, activeAllocation, cancellationToken);

        return new MessageResponse("Evrak başarıyla zimmetlendi");
    }
}
