using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.IncomingDocumentAttachmentFeatures.Queries.GetIncomingDocumentAttachmentFile;

public sealed class GetIncomingDocumentAttachmentFileHandler
    : IRequestHandler<GetIncomingDocumentAttachmentFileQuery, IncomingDocumentAttachmentFileDto?>
{
    private readonly IIncomingDocumentAttachmentService _attachmentService;
    private readonly IIncomingDocumentAttachmentStorageService _storageService;

    public GetIncomingDocumentAttachmentFileHandler(
        IIncomingDocumentAttachmentService attachmentService,
        IIncomingDocumentAttachmentStorageService storageService)
    {
        _attachmentService = attachmentService;
        _storageService = storageService;
    }

    public async Task<IncomingDocumentAttachmentFileDto?> Handle(
        GetIncomingDocumentAttachmentFileQuery request,
        CancellationToken cancellationToken)
    {
        var attachment = await _attachmentService.GetByIdAsync(request.Id, cancellationToken);
        if (attachment is null || !_storageService.Exists(attachment.StoredFileName))
            return null;

        return new IncomingDocumentAttachmentFileDto
        {
            Content = new FileStream(
                _storageService.GetFullPath(attachment.StoredFileName), FileMode.Open, FileAccess.Read, FileShare.Read),
            ContentType = "application/pdf",
            FileName = attachment.OriginalFileName
        };
    }
}
