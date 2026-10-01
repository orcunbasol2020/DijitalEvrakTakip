using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.DocumentAllocationRequestFeatures.Queries.GetPendingAllocationRequestsByUserId;

// Kullanıcının onayını bekleyen zimmet talepleri
public sealed record GetPendingAllocationRequestsByUserIdQuery(Guid UserId)
    : IRequest<IList<DocumentAllocationRequestDto>>;
