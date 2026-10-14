using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.AtlasTransferFeatures.Queries.GetAtlasTransferQueue;

/// <summary>Kuyrukta bekleyen ve o anda aktarılan evraklar.</summary>
public sealed record GetAtlasTransferQueueQuery() : IRequest<IReadOnlyList<AtlasTransferQueueItemDto>>;
