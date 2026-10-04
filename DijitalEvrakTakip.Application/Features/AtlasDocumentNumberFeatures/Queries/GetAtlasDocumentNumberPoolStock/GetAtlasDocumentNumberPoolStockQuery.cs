using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.AtlasDocumentNumberFeatures.Queries.GetAtlasDocumentNumberPoolStock;

public sealed record GetAtlasDocumentNumberPoolStockQuery() : IRequest<AtlasDocumentNumberPoolStockDto>;
