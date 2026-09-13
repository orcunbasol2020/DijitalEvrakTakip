using MediatR;
using DijitalEvrakTakip.Domain.Dtos;

namespace DijitalEvrakTakip.Application.Features.OutgoingDocumentAllocationFeatures.Commands.UploadWetSignedDocument
{
    public sealed record UploadWetSignedDocumentCommand(
        Guid OutgoingDocumentId,
        string FileName,
        byte[] FileContent,
        string? UploadedUserId
    ) : IRequest<MessageResponse>;
}
