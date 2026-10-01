using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.DocumentAllocationRequestFeatures.Commands.ApproveAllocationRequests;

public sealed class ApproveAllocationRequestsCommandHandler
    : IRequestHandler<ApproveAllocationRequestsCommand, MessageResponse>
{
    private readonly IDocumentAllocationRequestService _allocationRequestService;

    public ApproveAllocationRequestsCommandHandler(
        IDocumentAllocationRequestService allocationRequestService)
    {
        _allocationRequestService = allocationRequestService;
    }

    public Task<MessageResponse> Handle(
        ApproveAllocationRequestsCommand request,
        CancellationToken cancellationToken)
    {
        return AllocationRequestActionRunner.RunBulkAsync(
            _allocationRequestService,
            AllocationRequestAction.Approve,
            request.RequestIds,
            request.UserId,
            null,
            cancellationToken);
    }
}
