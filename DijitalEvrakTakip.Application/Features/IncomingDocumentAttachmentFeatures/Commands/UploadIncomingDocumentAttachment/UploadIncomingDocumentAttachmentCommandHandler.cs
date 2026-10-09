using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.IncomingDocumentAttachmentFeatures.Commands.UploadIncomingDocumentAttachment;

public sealed class UploadIncomingDocumentAttachmentCommandHandler
    : IRequestHandler<UploadIncomingDocumentAttachmentCommand, IncomingDocumentAttachmentDto>
{
    private const int NameMaxLength = 250;

    private readonly IIncomingDocumentService _incomingDocumentService;
    private readonly IIncomingDocumentAttachmentService _attachmentService;
    private readonly IIncomingDocumentAttachmentStorageService _storageService;

    public UploadIncomingDocumentAttachmentCommandHandler(
        IIncomingDocumentService incomingDocumentService,
        IIncomingDocumentAttachmentService attachmentService,
        IIncomingDocumentAttachmentStorageService storageService)
    {
        _incomingDocumentService = incomingDocumentService;
        _attachmentService = attachmentService;
        _storageService = storageService;
    }

    public async Task<IncomingDocumentAttachmentDto> Handle(
        UploadIncomingDocumentAttachmentCommand request,
        CancellationToken cancellationToken)
    {
        var document = await _incomingDocumentService.GetByIdAsync(request.IncomingDocumentId, cancellationToken);
        if (document is null || document.IsDeleted)
            throw new Exception("Evrak bulunamadı.");

        await using var contentStream = new MemoryStream(request.FileContent);
        var storedFileName = await _storageService.SaveAsync(document.Id, contentStream, cancellationToken);

        var originalFileName = Path.GetFileName(request.FileName);
        var attachment = new IncomingDocumentAttachment
        {
            IncomingDocumentId = document.Id,
            Name = ResolveName(request.Name, originalFileName),
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            OriginalFileName = originalFileName,
            StoredFileName = storedFileName,
            FileSize = request.FileContent.LongLength
        };

        try
        {
            await _attachmentService.CreateAsync(attachment, request.UserId, cancellationToken);
        }
        catch
        {
            _storageService.Delete(storedFileName);
            throw;
        }

        return IncomingDocumentAttachmentMapper.ToDto(attachment);
    }

    private static string ResolveName(string? name, string originalFileName)
    {
        var resolved = string.IsNullOrWhiteSpace(name)
            ? Path.GetFileNameWithoutExtension(originalFileName).Trim()
            : name.Trim();

        if (resolved.Length == 0)
            resolved = "Ek";

        return resolved.Length > NameMaxLength ? resolved[..NameMaxLength] : resolved;
    }
}
