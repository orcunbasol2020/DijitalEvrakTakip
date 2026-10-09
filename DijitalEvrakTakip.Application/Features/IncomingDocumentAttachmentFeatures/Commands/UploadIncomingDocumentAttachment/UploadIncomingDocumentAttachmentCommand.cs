using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.IncomingDocumentAttachmentFeatures.Commands.UploadIncomingDocumentAttachment;

/// <summary>
/// Gelen evraka elektronik ek (PDF) yükler. Ad verilmezse dosya adı (uzantısız) kullanılır.
/// </summary>
public sealed record UploadIncomingDocumentAttachmentCommand(
    Guid IncomingDocumentId,
    string FileName,
    byte[] FileContent,
    string? Name,
    string? Description,
    string UserId
) : IRequest<IncomingDocumentAttachmentDto>;
