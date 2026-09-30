using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.DocumentAllocationRequestFeatures.Commands.CancelAllocationRequests;

public sealed class CancelAllocationRequestsCommandHandler
    : IRequestHandler<CancelAllocationRequestsCommand, MessageResponse>
{
    private readonly IDocumentAllocationRequestService _allocationRequestService;

    public CancelAllocationRequestsCommandHandler(
        IDocumentAllocationRequestService allocationRequestService)
    {
        _allocationRequestService = allocationRequestService;
    }

    public Task<MessageResponse> Handle(
        CancelAllocationRequestsCommand request,
        CancellationToken cancellationToken)
    {
        return AllocationRequestActionRunner.RunBulkAsync(
            _allocationRequestService,
            AllocationRequestAction.Cancel,
            request.RequestIds,
            request.UserId,
            request.Note,
            cancellationToken);
    }
}
