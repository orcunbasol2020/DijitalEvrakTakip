using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.DocumentAllocationRequestFeatures.Commands.CancelAllocationRequests;

// Toplu iptal, tüm taleplere ortak gerekçeyle. Alıcıya kişi başına tek özet bildirim gider.
public sealed record CancelAllocationRequestsCommand(
    IList<Guid> RequestIds,
    string UserId,
    string? Note
) : IRequest<MessageResponse>;
