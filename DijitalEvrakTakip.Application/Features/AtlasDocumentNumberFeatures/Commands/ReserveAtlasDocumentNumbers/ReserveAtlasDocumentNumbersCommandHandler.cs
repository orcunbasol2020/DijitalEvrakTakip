using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.AtlasDocumentNumberFeatures.Commands.ReserveAtlasDocumentNumbers;

public sealed class ReserveAtlasDocumentNumbersCommandHandler
    : IRequestHandler<ReserveAtlasDocumentNumbersCommand, MessageResponse>
{
    private readonly IAtlasDocumentNumberPoolService _poolService;

    public ReserveAtlasDocumentNumbersCommandHandler(IAtlasDocumentNumberPoolService poolService)
    {
        _poolService = poolService;
    }

    public async Task<MessageResponse> Handle(
        ReserveAtlasDocumentNumbersCommand request,
        CancellationToken cancellationToken)
    {
        var numbers = await _poolService.ReserveAsync(Guid.Parse(request.UserId), request.Count, cancellationToken);

        if (numbers.Count == 0)
            throw new Exception("Havuzda boşta evrak numarası yok ve Atlas'tan numara alınamadı.");

        var message = numbers.Count < request.Count
            ? $"{request.Count} numara istendi, havuzda yalnızca {numbers.Count} numara vardı."
            : $"{numbers.Count} numara ayrıldı.";

        return new MessageResponse(message, numbers);
    }
}
