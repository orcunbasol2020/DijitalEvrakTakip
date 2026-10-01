using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.OutgoingDocumentAllocationFeatures.Queries.GetAllocationTransferCountByUserId;

public sealed record GetOutgoingAllocationTransferCountByUserIdQuery(Guid UserId)
    : IRequest<UserAllocationTransferCountDto>;
