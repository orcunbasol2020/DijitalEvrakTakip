using DijitalEvrakTakip.Application.Features.EnvelopeDocumentFeatures.Commands.CreateEnvelopeDocument;
using DijitalEvrakTakip.Application.Features.OutgoingDocumentFeatures.Commands.CreateOutgoingDocument;
using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;
using MediatR;

public sealed class CreateEnvelopeDocumentCommandHandler
    : IRequestHandler<CreateEnvelopeDocumentCommand, MessageResponse>
{
    private readonly IEnvelopeDocumentService _envelopeDocumentService;
    private readonly IOutgoingDocumentService _outgoingDocumentService;

    public CreateEnvelopeDocumentCommandHandler(
        IEnvelopeDocumentService envelopeDocumentService,
        IOutgoingDocumentService outgoingDocumentService)
    {
        _envelopeDocumentService = envelopeDocumentService;
        _outgoingDocumentService = outgoingDocumentService;
    }

    public async Task<MessageResponse> Handle(
        CreateEnvelopeDocumentCommand request,
        CancellationToken cancellationToken)
    {
        // Okutulan QR için OutgoingDocuments'ta zaten bir kayıt var mı bakılır (varsa ilk bulunan kullanılır);
        // yoksa yeni bir OutgoingDocument kaydı açılıp Id'si alınır.
        var existingDocument = await _outgoingDocumentService.GetByQrCodeAsync(
            request.QrCode,
            cancellationToken);

        Guid documentId;

        if (existingDocument is not null)
        {
            documentId = existingDocument.Id;
        }
        else
        {
            var createOutgoingDocumentCommand = new CreateOutgoingDocumentCommand(
                QrCode: request.QrCode,
                OriginalDocumentNumber: null,
                SecurityDegree: null,
                Type: null,
                LanguageId: null,
                Subject: null,
                Content_Ocr: null,
                Status: null,
                DepartmentId: null,
                ExternalInstitutonId: null,
                ElectronicCopy: null,
                EbysTransfer: null,
                PageCount: null,
                Notes: null,
                DocumentDate: null,
                CreatedUserId: request.CreatedUserId);

            documentId = await _outgoingDocumentService.CreateAsync(
                createOutgoingDocumentCommand,
                cancellationToken);
        }

        EnvelopeDocument envelopeDocument = new()
        {
            EnvelopeId = request.EnvelopeId,
            QrCode = request.QrCode,
            DocumentId = documentId,
        };

        await _envelopeDocumentService.CreateAsync(envelopeDocument, cancellationToken);

        // Dönüşe eklenen Id'yi ekliyoruz
        return new MessageResponse(
            Message: "Evrak zarfa başarıyla eklendi",
            Data: new { envelopeDocument.QrCode, envelopeDocument.Id, envelopeDocument.DocumentId } // frontend bunu alabilir
        );
    }
}