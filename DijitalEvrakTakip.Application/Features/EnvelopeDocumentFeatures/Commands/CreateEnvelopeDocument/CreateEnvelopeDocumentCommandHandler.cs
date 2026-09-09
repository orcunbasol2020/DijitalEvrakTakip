using DijitalEvrakTakip.Application.Features.EnvelopeDocumentFeatures.Commands.CreateEnvelopeDocument;
using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;
using MediatR;

public sealed class CreateEnvelopeDocumentCommandHandler
    : IRequestHandler<CreateEnvelopeDocumentCommand, MessageResponse>
{
    private readonly IEnvelopeDocumentService _envelopeDocumentService;

    public CreateEnvelopeDocumentCommandHandler(IEnvelopeDocumentService envelopeDocumentService)
    {
        _envelopeDocumentService = envelopeDocumentService;
    }

    public async Task<MessageResponse> Handle(
        CreateEnvelopeDocumentCommand request,
        CancellationToken cancellationToken)
    {
        EnvelopeDocument envelopeDocument = new()
        {
            EnvelopeId = request.EnvelopeId,
            QrCode = request.QrCode,
        };

        await _envelopeDocumentService.CreateAsync(envelopeDocument, cancellationToken);

        // Dönüşe eklenen Id'yi ekliyoruz
        return new MessageResponse(
            Message: "Evrak zarfa başarıyla eklendi",
            Data: new { envelopeDocument.QrCode, envelopeDocument.Id } // frontend bunu alabilir
        );
    }
}