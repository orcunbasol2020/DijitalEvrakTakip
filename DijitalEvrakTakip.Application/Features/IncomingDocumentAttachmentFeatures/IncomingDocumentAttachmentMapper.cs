using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;

namespace DijitalEvrakTakip.Application.Features.IncomingDocumentAttachmentFeatures;

internal static class IncomingDocumentAttachmentMapper
{
    public static IncomingDocumentAttachmentDto ToDto(IncomingDocumentAttachment x) => new(
        x.Id,
        x.IncomingDocumentId,
        x.Name,
        x.Description,
        x.OrderNo,
        x.OriginalFileName,
        x.FileSize,
        x.UploadedUserId,
        x.CreatedDate,
        x.UpdateDate
    );
}
