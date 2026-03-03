using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.ScannedDocumentFeatures.Commands.CreateScannedDocument
{
    public sealed record CreateScannedDocumentCommand(
        string FileName,
        string OriginalPath,
        string NewPath,
        string DocumentNumber
    ) : IRequest<MessageResponse>;
}