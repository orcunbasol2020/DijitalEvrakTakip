using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.ScannedDocumentFeatures.Commands.UpdateScannedDocument
{
    public sealed record UpdateScannedDocumentCommand(
        Guid Id,
        string DocumentNumber,
        string UserId
    ) : IRequest<MessageResponse>;
}