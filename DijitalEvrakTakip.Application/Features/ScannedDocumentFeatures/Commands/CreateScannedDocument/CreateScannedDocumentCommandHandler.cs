using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;
using MediatR;
using Mapster;

namespace DijitalEvrakTakip.Application.Features.ScannedDocumentFeatures.Commands.CreateScannedDocument;

public sealed class CreateScannedDocumentCommandHandler
    : IRequestHandler<CreateScannedDocumentCommand, MessageResponse>
{
    private readonly IScannedDocumentService _scannedDocumentService;

    public CreateScannedDocumentCommandHandler(IScannedDocumentService scannedDocumentService)
    {
        _scannedDocumentService = scannedDocumentService;
    }

    public async Task<MessageResponse> Handle(
        CreateScannedDocumentCommand request,
        CancellationToken cancellationToken)
    {
        ScannedDocument scannedDocument = request.Adapt<ScannedDocument>();

        await _scannedDocumentService.CreateAsync(scannedDocument, cancellationToken);

        return new("Belge Başarıyla Kaydedildi");
    }
}