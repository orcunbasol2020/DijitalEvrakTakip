using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.IncomingDocumentAttachmentFeatures.Queries.GetIncomingDocumentAttachments;

public sealed record GetIncomingDocumentAttachmentsQuery(
    Guid IncomingDocumentId
) : IRequest<IList<IncomingDocumentAttachmentDto>>;
