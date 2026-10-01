using MediatR;
using DijitalEvrakTakip.Domain.Dtos;

namespace DijitalEvrakTakip.Application.Features.OutgoingDocumentAllocationFeatures.Commands.CreateOutgoingDocumentAllocation
{
    public sealed record CreateOutgoingDocumentAllocationCommand(
        Guid OutgoingDocumentId,
        string UserId,
        int UserType,
        string CreatedUserId,
        string Status
    ) : IRequest<MessageResponse>;
}
