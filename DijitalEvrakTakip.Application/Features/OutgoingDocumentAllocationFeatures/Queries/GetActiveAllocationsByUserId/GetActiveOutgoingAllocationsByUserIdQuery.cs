using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.OutgoingDocumentAllocationFeatures.Queries.GetActiveAllocationsByUserId;

public sealed record GetActiveOutgoingAllocationsByUserIdQuery(Guid UserId)
    : IRequest<IList<UserActiveOutgoingAllocationDto>>;
