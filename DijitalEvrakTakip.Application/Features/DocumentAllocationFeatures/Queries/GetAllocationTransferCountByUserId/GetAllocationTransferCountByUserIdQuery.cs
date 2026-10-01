using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.DocumentAllocationFeatures.Queries.GetAllocationTransferCountByUserId;

public sealed record GetAllocationTransferCountByUserIdQuery(Guid UserId)
    : IRequest<UserAllocationTransferCountDto>;
