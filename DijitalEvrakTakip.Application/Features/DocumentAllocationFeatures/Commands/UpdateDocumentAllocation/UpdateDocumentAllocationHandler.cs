using MediatR;
using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;

namespace DijitalEvrakTakip.Application.Features.DocumentAllocationFeatures.Commands.UpdateDocumentAllocation;

public sealed class UpdateDocumentAllocationCommandHandler
    : IRequestHandler<UpdateDocumentAllocationCommand, MessageResponse>
{
    private readonly IDocumentAllocationService _allocationService;

    public UpdateDocumentAllocationCommandHandler(
        IDocumentAllocationService allocationService)
    {
        _allocationService = allocationService;
    }

    public async Task<MessageResponse> Handle(
        UpdateDocumentAllocationCommand request,
        CancellationToken cancellationToken)
    {
        var allocation = await _allocationService
            .GetByIdAsync(request.Id, cancellationToken);

        if (allocation is null)
            return new MessageResponse("Zimmet kaydı bulunamadı");

        if (request.IncomingDocumentId.HasValue)
            allocation.IncomingDocumentId = request.IncomingDocumentId.Value;

        if (!string.IsNullOrWhiteSpace(request.UserId))
            allocation.UserId = Guid.Parse(request.UserId);

        if (request.Status.HasValue)
            allocation.Status = request.Status.Value;

        if (request.IsActive.HasValue)
            allocation.IsActive = request.IsActive.Value;

        await _allocationService.UpdateAsync(allocation, cancellationToken);

        return new MessageResponse("Zimmet kaydı güncellendi");
    }
}