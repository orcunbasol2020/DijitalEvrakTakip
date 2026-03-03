using MediatR;
using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;

namespace DijitalEvrakTakip.Application.Features.ScannedDocumentFeatures.Commands.UpdateScannedDocument
{
    public sealed class UpdateScannedDocumentNumberCommandHandler
        : IRequestHandler<UpdateScannedDocumentCommand, MessageResponse>
    {
        private readonly IScannedDocumentService _scannedDocumentService;

        public UpdateScannedDocumentNumberCommandHandler(IScannedDocumentService scannedDocumentService)
        {
            _scannedDocumentService = scannedDocumentService;
        }

        public async Task<MessageResponse> Handle(
            UpdateScannedDocumentCommand request,
            CancellationToken cancellationToken)
        {
            // ID'nin geçerli olup olmadığını kontrol et
            if (request.Id == Guid.Empty)
                throw new ArgumentException("Geçersiz belge Id.");

            // Belgeyi güncelle
            await _scannedDocumentService.UpdateDocumentNumberAsync(request, cancellationToken);

            return new MessageResponse("Belge numarası başarıyla güncellendi.");
        }
    }
}