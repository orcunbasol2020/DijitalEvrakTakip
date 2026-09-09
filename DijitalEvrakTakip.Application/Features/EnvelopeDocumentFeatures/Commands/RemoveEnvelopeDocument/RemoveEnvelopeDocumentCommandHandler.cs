using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.EnvelopeDocumentFeatures.Commands.RemoveEnvelopeDocument;

public sealed class RemoveEnvelopeDocumentCommandHandler
    : IRequestHandler<RemoveEnvelopeDocumentCommand, MessageResponse>
{
    private readonly IEnvelopeDocumentService _envelopeDocumentService;

    public RemoveEnvelopeDocumentCommandHandler(IEnvelopeDocumentService envelopeDocumentService)
    {
        _envelopeDocumentService = envelopeDocumentService;
    }

    public async Task<MessageResponse> Handle(
        RemoveEnvelopeDocumentCommand request,
        CancellationToken cancellationToken)
    {
        await _envelopeDocumentService.RemoveAsync(request.Id, cancellationToken);

        return new("Evrak zarf içinden çıkarıldı");
    }
}