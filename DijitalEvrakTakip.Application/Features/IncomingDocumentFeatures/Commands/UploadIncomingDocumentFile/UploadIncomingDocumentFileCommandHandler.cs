using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Commands.UploadIncomingDocumentFile;

public sealed class UploadIncomingDocumentFileCommandHandler
    : IRequestHandler<UploadIncomingDocumentFileCommand, MessageResponse>
{
    private readonly IIncomingDocumentService _incomingDocumentService;
    private readonly IIncomingDocumentStorageService _storageService;

    public UploadIncomingDocumentFileCommandHandler(
        IIncomingDocumentService incomingDocumentService,
        IIncomingDocumentStorageService storageService)
    {
        _incomingDocumentService = incomingDocumentService;
        _storageService = storageService;
    }

    public async Task<MessageResponse> Handle(
        UploadIncomingDocumentFileCommand request,
        CancellationToken cancellationToken)
    {
        var document = await _incomingDocumentService.GetByIdAsync(
            request.IncomingDocumentId, cancellationToken);

        if (document is null || document.IsDeleted)
            throw new Exception("Evrak bulunamadı.");

        var previousFileName = document.DocumentName;

        await using var contentStream = new MemoryStream(request.FileContent);

        var savedFileName = await _storageService.SaveAsync(
            document.QrCode,
            request.FileName,
            contentStream,
            cancellationToken);

        try
        {
            await _incomingDocumentService.AttachUploadedFileAsync(
                document.Id,
                savedFileName,
                _storageService.GetFullPath(savedFileName),
                request.FileName,
                request.UserId,
                cancellationToken);
        }
        catch
        {
            _storageService.Delete(savedFileName);
            throw;
        }

        if (!string.IsNullOrEmpty(previousFileName) && previousFileName != savedFileName)
            _storageService.Delete(previousFileName);

        return new MessageResponse("Evrak dosyası başarıyla yüklendi.");
    }
}
