using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Commands.PreRegisterIncomingDocument;

public sealed record PreRegisterIncomingDocumentCommand
(
    string QrCode,
    string UserId,
    int DocumentDirection
) : IRequest<string>;
