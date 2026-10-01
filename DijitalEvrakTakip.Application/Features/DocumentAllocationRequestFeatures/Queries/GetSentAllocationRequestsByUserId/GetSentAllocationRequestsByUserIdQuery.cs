using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.DocumentAllocationRequestFeatures.Queries.GetSentAllocationRequestsByUserId;

// Kullanıcının devrettiği veya işlemini yaptığı zimmet talepleri
public sealed record GetSentAllocationRequestsByUserIdQuery(Guid UserId, bool OnlyPending)
    : IRequest<IList<DocumentAllocationRequestDto>>;
