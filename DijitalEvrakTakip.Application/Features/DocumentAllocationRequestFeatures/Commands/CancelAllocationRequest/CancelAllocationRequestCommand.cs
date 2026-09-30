using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.DocumentAllocationRequestFeatures.Commands.CancelAllocationRequest;

// Devreden veya işlemi yapan kullanıcı bekleyen talebi geri çeker; evrak devredende kalır
public sealed record CancelAllocationRequestCommand(
    Guid RequestId,
    string UserId,
    string? Note
) : IRequest<MessageResponse>;
