using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.AtlasTransferFeatures.Queries.GetAtlasTransferStats;

/// <summary>Yayın (Atlas aktarım) durumlarına göre gelen evrak sayıları.</summary>
public sealed record GetAtlasTransferStatsQuery() : IRequest<AtlasTransferStatsDto>;
