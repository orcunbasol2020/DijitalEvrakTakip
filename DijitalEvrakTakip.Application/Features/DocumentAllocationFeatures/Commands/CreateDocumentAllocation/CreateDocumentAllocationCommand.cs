using MediatR;
using DijitalEvrakTakip.Domain.Dtos;

namespace DijitalEvrakTakip.Application.Features.DocumentAllocationFeatures.Commands.CreateDocumentAllocation
{
    public sealed record CreateDocumentAllocationCommand(
        Guid IncomingDocumentId,
        string UserId,
        int UserType,
        string CreatedUserId,
        string Status
    ) : IRequest<MessageResponse>;
}