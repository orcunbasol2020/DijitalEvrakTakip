using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.ExternalUserFeatures.Commands.UpdateExternalUser;

public sealed record UpdateExternalUserCommand(
    Guid Id,
    string? Name,
    string? Surname,
    string? Email,
    string? IdentityNo,
    int? UserType,
    Guid? ExternalInstitutionId,
    bool? IsActive
) : IRequest<MessageResponse>;
