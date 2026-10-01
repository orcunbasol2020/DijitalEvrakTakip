using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.DocumentAllocationRequestFeatures.Commands.RejectAllocationRequest;

// Alıcı zimmeti kabul etmez; evrak devredende kalır
public sealed record RejectAllocationRequestCommand(
    Guid RequestId,
    string UserId,
    string? Note
) : IRequest<MessageResponse>;
