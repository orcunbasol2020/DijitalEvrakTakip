using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.DocumentAssignmentFeatures.Commands.CreateDocumentAssignment
{
    public sealed record CreateDocumentAssignmentCommand(
        Guid DocumentId,
        string UserId,
        bool? Lock,
        bool? IsActive
    ) : IRequest<MessageResponse>;
}