using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Enums;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.DocumentAllocationFeatures.Commands.CreateDocumentAllocation;

public sealed class CreateDocumentAllocationCommandHandler
    : IRequestHandler<CreateDocumentAllocationCommand, MessageResponse>
{
    private readonly IDocumentAllocationService _allocationService;
    private readonly IDocumentAllocationRequestService _allocationRequestService;

    public CreateDocumentAllocationCommandHandler(
        IDocumentAllocationService allocationService,
        IDocumentAllocationRequestService allocationRequestService)
    {
        _allocationService = allocationService;
        _allocationRequestService = allocationRequestService;
    }

    public async Task<MessageResponse> Handle(
        CreateDocumentAllocationCommand request,
        CancellationToken cancellationToken)
    {
        if (!int.TryParse(request.Status, out var status) ||
            !Enum.IsDefined(typeof(AllocationStatusEnum), status))
            return new MessageResponse(
                "Geçersiz Status: 1 (Ön Kayıt), 2 (Devir), 3 (Teslim), 4 (Arşiv), 5 (Teslim Alındı), 6 (Kargoya Verildi) veya 7 (Devir Alındı) olmalıdır.");

        var userId = Guid.Parse(request.UserId);
        var createdUserId = Guid.Parse(request.CreatedUserId);

        // Onay bekleyen talep varken zimmet değiştirilemez; önce talep iptal edilmeli
        var pendingRequest = await _allocationRequestService
            .GetPendingDtoByDocumentIdAsync(request.IncomingDocumentId, cancellationToken);

        if (pendingRequest is not null)
            return new MessageResponse(
                $"Bu evrak için {pendingRequest.ToUserFullName} kullanıcısının zimmet onayı bekleniyor. Yeni zimmet için önce bekleyen talep iptal edilmelidir.",
                pendingRequest);

        var activeAllocation =
            await _allocationService.GetActiveByDocumentIdAsync(
                request.IncomingDocumentId,
                cancellationToken);

        // Aktif zimmet aynı kişideyse engelle; başka kişideyse CreateAsync pasife çeker
        if (activeAllocation != null && activeAllocation.UserId == userId)
            return new MessageResponse("Bu evrak zaten bu kullanıcıya zimmetli");

        var toOtherInternalUser =
            request.UserType == (int)AllocationUserTypeEnum.Internal &&
            userId != createdUserId;

        // Başka bir iç kullanıcı adına "alındı" zimmeti açılamaz; bu statüler onayla veya Teslim Al ile oluşur
        if (toOtherInternalUser &&
            (status == (int)AllocationStatusEnum.TeslimAlindi || status == (int)AllocationStatusEnum.DevirAlindi))
            return new MessageResponse(
                "Teslim Alındı ve Devir Alındı zimmetleri başka bir kullanıcı adına açılamaz. Teslim Et veya Devir Et kullanın; zimmet alıcı onaylayınca bu statüye geçer.");

        // Kurum içi başka kullanıcıya Devir / Teslim: alıcı onaylayana kadar zimmet devredende kalır
        var requiresApproval =
            (status == (int)AllocationStatusEnum.Devir || status == (int)AllocationStatusEnum.Teslim) &&
            toOtherInternalUser &&
            await _allocationRequestService.IsApprovalRequiredAsync(cancellationToken);

        if (requiresApproval)
        {
            var allocationRequest = new DocumentAllocationRequest
            {
                IncomingDocumentId = request.IncomingDocumentId,
                FromAllocationId = activeAllocation?.Id,
                FromUserId = activeAllocation?.UserId,
                ToUserId = userId,
                RequestedAllocationStatus = status,
                RequestedByUserId = createdUserId
            };

            if (!await _allocationRequestService.CreateAsync(allocationRequest, cancellationToken))
                return new MessageResponse(
                    "Bu evrak için az önce başka bir zimmet talebi oluşturuldu. Sayfayı yenileyip tekrar deneyin.");

            return new MessageResponse(
                "Zimmet talebi alıcının onayına gönderildi. Onay verilene kadar evrak mevcut zimmet sahibinde kalır.",
                new { RequestId = allocationRequest.Id, IsPendingApproval = true });
        }

        // Yeni aktif zimmet oluştur
        var allocation = new DocumentAllocation
        {
            IncomingDocumentId = request.IncomingDocumentId,
            UserId = userId,
            UserType = request.UserType,
            CreatedUserId = createdUserId,
            Status = status,
            Source = (int)AllocationSourceEnum.EvrakTakip,
            IsActive = true
        };

        await _allocationService.CreateAsync(allocation, activeAllocation, cancellationToken);

        return new MessageResponse("Evrak başarıyla zimmetlendi");
    }
}
