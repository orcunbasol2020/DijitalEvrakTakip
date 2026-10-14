using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.AtlasTransferFeatures.Commands.RetryAtlasTransfer;

public sealed class RetryAtlasTransferCommandHandler
    : IRequestHandler<RetryAtlasTransferCommand, MessageResponse>
{
    private readonly IAtlasTransferService _transferService;

    public RetryAtlasTransferCommandHandler(IAtlasTransferService transferService)
    {
        _transferService = transferService;
    }

    public async Task<MessageResponse> Handle(
        RetryAtlasTransferCommand request,
        CancellationToken cancellationToken)
    {
        await _transferService.RetryAsync(request.DocumentId, request.UserId, cancellationToken);

        return new MessageResponse("Evrak yeniden Atlas aktarım sırasına alındı.");
    }
}
