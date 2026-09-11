using MediatR;
using DijitalEvrakTakip.Domain.Dtos;

namespace DijitalEvrakTakip.Application.Features.OutgoingDocumentAllocationFeatures.Commands.UpdateOutgoingDocumentAllocation;

public sealed record UpdateOutgoingDocumentAllocationCommand
(
    Guid Id,
    Guid? OutgoingDocumentId,
    string? UserId,
    int? UserType,
    int? Status,
    bool? IsActive
) : IRequest<MessageResponse>;
