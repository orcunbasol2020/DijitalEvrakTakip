using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.DocumentAllocationRequestFeatures.Commands.RejectAllocationRequest;

public sealed class RejectAllocationRequestCommandHandler
    : IRequestHandler<RejectAllocationRequestCommand, MessageResponse>
{
    private readonly IDocumentAllocationRequestService _allocationRequestService;

    public RejectAllocationRequestCommandHandler(
        IDocumentAllocationRequestService allocationRequestService)
    {
        _allocationRequestService = allocationRequestService;
    }

    public Task<MessageResponse> Handle(
        RejectAllocationRequestCommand request,
        CancellationToken cancellationToken)
    {
        return AllocationRequestActionRunner.RunSingleAsync(
            _allocationRequestService,
            AllocationRequestAction.Reject,
            request.RequestId,
            request.UserId,
            request.Note,
            cancellationToken);
    }
}
