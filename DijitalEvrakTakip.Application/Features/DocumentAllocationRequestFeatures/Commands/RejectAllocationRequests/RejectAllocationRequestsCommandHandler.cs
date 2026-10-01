using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.DocumentAllocationRequestFeatures.Commands.RejectAllocationRequests;

public sealed class RejectAllocationRequestsCommandHandler
    : IRequestHandler<RejectAllocationRequestsCommand, MessageResponse>
{
    private readonly IDocumentAllocationRequestService _allocationRequestService;

    public RejectAllocationRequestsCommandHandler(
        IDocumentAllocationRequestService allocationRequestService)
    {
        _allocationRequestService = allocationRequestService;
    }

    public Task<MessageResponse> Handle(
        RejectAllocationRequestsCommand request,
        CancellationToken cancellationToken)
    {
        return AllocationRequestActionRunner.RunBulkAsync(
            _allocationRequestService,
            AllocationRequestAction.Reject,
            request.RequestIds,
            request.UserId,
            request.Note,
            cancellationToken);
    }
}
