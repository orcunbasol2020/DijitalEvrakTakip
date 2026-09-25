using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Commands.UploadIncomingDocumentFile;

/// <summary>
/// Tarama gerektirmeyen bir gelen evrak için PDF dosyasını elle yükler.
/// Evrakta zaten bir dosya varsa üzerine yazılır.
/// </summary>
public sealed record UploadIncomingDocumentFileCommand(
    Guid IncomingDocumentId,
    string FileName,
    byte[] FileContent,
    string UserId
) : IRequest<MessageResponse>;
