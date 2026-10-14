using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.IncomingDocumentAttachmentFeatures.Queries.GetIncomingDocumentAttachmentFile;

/// <summary>Ek dosyasını getirir; ek veya dosyası yoksa null döner.</summary>
public sealed record GetIncomingDocumentAttachmentFileQuery(
    Guid Id
) : IRequest<IncomingDocumentAttachmentFileDto?>;
