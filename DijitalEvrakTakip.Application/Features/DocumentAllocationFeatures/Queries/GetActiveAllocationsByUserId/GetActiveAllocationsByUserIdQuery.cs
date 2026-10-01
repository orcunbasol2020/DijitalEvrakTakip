using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.DocumentAllocationFeatures.Queries.GetActiveAllocationsByUserId;

public sealed record GetActiveAllocationsByUserIdQuery(Guid UserId)
    : IRequest<IList<UserActiveAllocationDto>>;
