using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.AtlasDocumentNumberFeatures.Commands.CancelAtlasDocumentNumber;

public sealed class CancelAtlasDocumentNumberCommandHandler
    : IRequestHandler<CancelAtlasDocumentNumberCommand, MessageResponse>
{
    private readonly IAtlasDocumentNumberPoolService _poolService;

    public CancelAtlasDocumentNumberCommandHandler(IAtlasDocumentNumberPoolService poolService)
    {
        _poolService = poolService;
    }

    public async Task<MessageResponse> Handle(
        CancelAtlasDocumentNumberCommand request,
        CancellationToken cancellationToken)
    {
        var qrCode = request.QrCode.Trim();
        Guid? userId = Guid.TryParse(request.UserId, out var parsed) ? parsed : null;

        await _poolService.CancelAsync(qrCode, userId, request.Reason, cancellationToken);

        return new MessageResponse($"{qrCode} numarası iptal edildi.");
    }
}
