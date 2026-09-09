using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.ExternalUserFeatures.Commands.CreateExternalUser;

public sealed record CreateExternalUserCommand(
    string Name,
    string Surname,
    string Email,
    string IdentityNo,
    int UserType,
    Guid ExternalInstitutionId,
    bool IsActive
) : IRequest<MessageResponse>;