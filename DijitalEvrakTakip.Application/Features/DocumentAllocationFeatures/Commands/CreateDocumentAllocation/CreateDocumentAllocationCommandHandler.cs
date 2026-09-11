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

    public CreateDocumentAllocationCommandHandler(
        IDocumentAllocationService allocationService)
    {
        _allocationService = allocationService;
    }

    public async Task<MessageResponse> Handle(
        CreateDocumentAllocationCommand request,
        CancellationToken cancellationToken)
    {
        var activeAllocation =
            await _allocationService.GetActiveByDocumentIdAsync(
                request.IncomingDocumentId,
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
        var allocation = new DocumentAllocation
        {
            IncomingDocumentId = request.IncomingDocumentId,
            UserId = Guid.Parse(request.UserId),
            UserType = request.UserType,
            CreatedUserId = Guid.Parse(request.CreatedUserId),
            Status = Convert.ToInt32(request.Status),
            Source = (int)AllocationSourceEnum.EvrakTakip,
            IsActive = true
        };

        await _allocationService.CreateAsync(allocation, cancellationToken);

        return new MessageResponse("Evrak başarıyla zimmetlendi");
    }
}