using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.DocumentAllocationRequestFeatures.Commands.ApproveAllocationRequest;

public sealed class ApproveAllocationRequestCommandHandler
    : IRequestHandler<ApproveAllocationRequestCommand, MessageResponse>
{
    private readonly IDocumentAllocationRequestService _allocationRequestService;

    public ApproveAllocationRequestCommandHandler(
        IDocumentAllocationRequestService allocationRequestService)
    {
        _allocationRequestService = allocationRequestService;
    }

    public Task<MessageResponse> Handle(
        ApproveAllocationRequestCommand request,
        CancellationToken cancellationToken)
    {
        return AllocationRequestActionRunner.RunSingleAsync(
            _allocationRequestService,
            AllocationRequestAction.Approve,
            request.RequestId,
            request.UserId,
            request.Note,
            cancellationToken,
            request.HasDiscrepancy);
    }
}
