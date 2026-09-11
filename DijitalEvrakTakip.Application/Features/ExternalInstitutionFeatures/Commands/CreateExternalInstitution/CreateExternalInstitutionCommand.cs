using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.ExternalInstitutionFeatures.Commands.CreateExternalInstitution;

public sealed record CreateExternalInstitutionCommand(
    string Name,
    int Type,
    string? Address = null,
    Guid? ParentId = null
) : IRequest<MessageResponse>;
