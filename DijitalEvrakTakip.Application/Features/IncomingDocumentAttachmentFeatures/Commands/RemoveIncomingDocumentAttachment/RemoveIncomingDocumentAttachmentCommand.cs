using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.IncomingDocumentAttachmentFeatures.Commands.RemoveIncomingDocumentAttachment;

public sealed record RemoveIncomingDocumentAttachmentCommand(
    Guid Id,
    string UserId
) : IRequest<MessageResponse>;
