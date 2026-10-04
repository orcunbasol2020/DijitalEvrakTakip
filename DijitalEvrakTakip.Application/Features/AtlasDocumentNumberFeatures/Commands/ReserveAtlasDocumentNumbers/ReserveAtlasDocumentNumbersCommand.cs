using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.AtlasDocumentNumberFeatures.Commands.ReserveAtlasDocumentNumbers;

/// <summary>
/// Etiket basmak için havuzdan numara ayırır.
/// </summary>
public sealed record ReserveAtlasDocumentNumbersCommand(
    string UserId,
    int Count = 1
) : IRequest<MessageResponse>;
