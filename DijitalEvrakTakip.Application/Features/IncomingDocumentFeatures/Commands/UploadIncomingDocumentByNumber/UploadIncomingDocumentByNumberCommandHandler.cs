using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Commands.UploadIncomingDocumentByNumber;

public sealed class UploadIncomingDocumentByNumberCommandHandler
    : IRequestHandler<UploadIncomingDocumentByNumberCommand, MessageResponse>
{
    private readonly IIncomingDocumentService _incomingDocumentService;
    private readonly IIncomingDocumentStorageService _storageService;

    public UploadIncomingDocumentByNumberCommandHandler(
        IIncomingDocumentService incomingDocumentService,
        IIncomingDocumentStorageService storageService)
    {
        _incomingDocumentService = incomingDocumentService;
        _storageService = storageService;
    }

    public async Task<MessageResponse> Handle(
        UploadIncomingDocumentByNumberCommand request,
        CancellationToken cancellationToken)
    {
        var documentNumber = request.DocumentNumber.Trim();

        await using var contentStream = new MemoryStream(request.FileContent);

        var savedFileName = await _storageService.SaveAsync(
            documentNumber,
            request.FileName,
            contentStream,
            cancellationToken);

        try
        {
            await _incomingDocumentService.AttachUploadedFileByNumberAsync(
                documentNumber,
                savedFileName,
                _storageService.GetFullPath(savedFileName),
                request.FileName,
                request.UserId,
                cancellationToken);
        }
        catch
        {
            // Kayıt yapılamadıysa (numara kullanılmış olabilir) kaydedilen dosya geri alınır
            _storageService.Delete(savedFileName);
            throw;
        }

        return new MessageResponse("Evrak dosyası başarıyla yüklendi.");
    }
}
