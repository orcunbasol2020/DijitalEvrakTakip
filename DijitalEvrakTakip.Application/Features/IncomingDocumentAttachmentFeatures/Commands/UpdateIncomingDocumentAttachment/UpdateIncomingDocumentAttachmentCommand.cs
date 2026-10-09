using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.IncomingDocumentAttachmentFeatures.Commands.UpdateIncomingDocumentAttachment;

/// <summary>
/// Ekin adını, açıklamasını veya EYP'deki sırasını günceller. null gönderilen alan korunur;
/// boş açıklama mevcut açıklamayı temizler.
/// </summary>
public sealed record UpdateIncomingDocumentAttachmentCommand(
    Guid Id,
    string? Name,
    string? Description,
    int? OrderNo
) : IRequest<IncomingDocumentAttachmentDto>;
