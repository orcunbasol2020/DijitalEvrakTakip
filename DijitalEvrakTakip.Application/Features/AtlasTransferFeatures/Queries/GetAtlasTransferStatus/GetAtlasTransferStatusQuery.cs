using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.AtlasTransferFeatures.Queries.GetAtlasTransferStatus;

/// <summary>Aktarım servisinin durumu: ayarlar, son/sonraki tur, ön koşul kontrolleri ve sayılar.</summary>
public sealed record GetAtlasTransferStatusQuery() : IRequest<AtlasTransferStatusDto>;
