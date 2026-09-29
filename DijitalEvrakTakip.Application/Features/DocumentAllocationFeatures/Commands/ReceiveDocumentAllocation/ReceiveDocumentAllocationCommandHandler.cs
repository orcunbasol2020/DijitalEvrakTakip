using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Enums;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.DocumentAllocationFeatures.Commands.ReceiveDocumentAllocation;

public sealed class ReceiveDocumentAllocationCommandHandler
    : IRequestHandler<ReceiveDocumentAllocationCommand, MessageResponse>
{
    private readonly IDocumentAllocationService _allocationService;

    public ReceiveDocumentAllocationCommandHandler(
        IDocumentAllocationService allocationService)
    {
        _allocationService = allocationService;
    }

    public async Task<MessageResponse> Handle(
        ReceiveDocumentAllocationCommand request,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(request.UserId, out var userId))
            return new MessageResponse("Geçersiz UserId");

        var allocation = await _allocationService
            .GetActiveByDocumentIdAsync(request.IncomingDocumentId, cancellationToken);

        if (allocation is null)
            return new MessageResponse("Evrağın aktif zimmeti bulunamadı");

        if (allocation.UserId != userId)
            return new MessageResponse("Evrak bu kullanıcıya zimmetli değil");

        if (allocation.Status == (int)AllocationStatusEnum.TeslimAlindi)
            return new MessageResponse("Evrak zaten teslim alınmış");

        await _allocationService.ReceiveAsync(allocation, cancellationToken);

        return new MessageResponse("Evrak teslim alındı");
    }
}
