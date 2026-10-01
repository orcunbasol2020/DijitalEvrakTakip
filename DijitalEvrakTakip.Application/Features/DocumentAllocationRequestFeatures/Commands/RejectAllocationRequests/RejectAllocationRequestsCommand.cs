using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.DocumentAllocationRequestFeatures.Commands.RejectAllocationRequests;

// Toplu red, tüm taleplere ortak gerekçeyle. Devredene kişi başına tek özet bildirim gider.
public sealed record RejectAllocationRequestsCommand(
    IList<Guid> RequestIds,
    string UserId,
    string? Note
) : IRequest<MessageResponse>;
