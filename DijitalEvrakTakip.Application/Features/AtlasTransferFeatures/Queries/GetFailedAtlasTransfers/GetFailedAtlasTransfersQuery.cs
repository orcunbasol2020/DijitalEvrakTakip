using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.AtlasTransferFeatures.Queries.GetFailedAtlasTransfers;

/// <summary>Atlas aktarımı hatalı olan evraklar ve son hata mesajları.</summary>
public sealed record GetFailedAtlasTransfersQuery() : IRequest<IReadOnlyList<AtlasTransferFailedDocumentDto>>;
