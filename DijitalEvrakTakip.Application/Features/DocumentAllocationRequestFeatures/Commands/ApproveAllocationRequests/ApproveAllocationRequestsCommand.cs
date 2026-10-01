using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.DocumentAllocationRequestFeatures.Commands.ApproveAllocationRequests;

// Toplu onay; her talep ayrı commit'le onaylanır, sonuçlar Data içinde talep bazında döner.
// Devredene talep başına değil, kişi başına tek özet bildirim gider.
public sealed record ApproveAllocationRequestsCommand(
    IList<Guid> RequestIds,
    string UserId
) : IRequest<MessageResponse>;
