using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.EnvelopeDocumentFeatures.Commands.RemoveEnvelopeDocument
{
    public sealed record RemoveEnvelopeDocumentCommand(
        Guid Id
    ) : IRequest<MessageResponse>;
}