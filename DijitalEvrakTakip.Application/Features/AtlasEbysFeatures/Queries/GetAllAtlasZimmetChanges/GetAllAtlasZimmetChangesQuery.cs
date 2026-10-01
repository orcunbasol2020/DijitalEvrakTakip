using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.AtlasEbysFeatures.Queries.GetAllAtlasZimmetChanges;

public sealed record GetAllAtlasZimmetChangesQuery()
    : IRequest<IList<AtlasZimmetChangeDto>>;
