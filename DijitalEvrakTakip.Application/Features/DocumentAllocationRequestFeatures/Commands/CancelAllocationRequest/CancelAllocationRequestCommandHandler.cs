using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.DocumentAllocationRequestFeatures.Commands.CancelAllocationRequest;

public sealed class CancelAllocationRequestCommandHandler
    : IRequestHandler<CancelAllocationRequestCommand, MessageResponse>
{
    private readonly IDocumentAllocationRequestService _allocationRequestService;

    public CancelAllocationRequestCommandHandler(
        IDocumentAllocationRequestService allocationRequestService)
    {
        _allocationRequestService = allocationRequestService;
    }

    public Task<MessageResponse> Handle(
        CancelAllocationRequestCommand request,
        CancellationToken cancellationToken)
    {
        return AllocationRequestActionRunner.RunSingleAsync(
            _allocationRequestService,
            AllocationRequestAction.Cancel,
            request.RequestId,
            request.UserId,
            request.Note,
            cancellationToken);
    }
}
