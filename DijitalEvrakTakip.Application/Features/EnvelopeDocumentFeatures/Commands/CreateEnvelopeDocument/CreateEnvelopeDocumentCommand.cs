using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.EnvelopeDocumentFeatures.Commands.CreateEnvelopeDocument
{
    public sealed record CreateEnvelopeDocumentCommand(
        Guid EnvelopeId,
        string QrCode
    ) : IRequest<MessageResponse>;
}