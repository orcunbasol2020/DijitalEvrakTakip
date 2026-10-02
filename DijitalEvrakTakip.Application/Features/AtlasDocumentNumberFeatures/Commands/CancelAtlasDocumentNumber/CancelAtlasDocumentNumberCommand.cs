using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.AtlasDocumentNumberFeatures.Commands.CancelAtlasDocumentNumber;

/// <summary>
/// Hatalı basılan veya kullanılmayacak numarayı iptal eder.
/// </summary>
public sealed record CancelAtlasDocumentNumberCommand(
    string QrCode,
    string? UserId,
    string? Reason
) : IRequest<MessageResponse>;
