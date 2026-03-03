using MediatR;
using DijitalEvrakTakip.Domain.Dtos;

namespace DijitalEvrakTakip.Application.Features.DocumentAllocationFeatures.Commands.UpdateDocumentAllocation;

public sealed record UpdateDocumentAllocationCommand
(
    Guid Id,
    Guid? IncomingDocumentId,
    string? UserId,
    int? Status,
    bool? IsActive
) : IRequest<MessageResponse>;