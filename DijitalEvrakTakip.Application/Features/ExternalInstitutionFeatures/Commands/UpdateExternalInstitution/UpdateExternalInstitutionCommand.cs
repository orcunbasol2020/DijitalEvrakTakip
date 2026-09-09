using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.ExternalInstitutionFeatures.Commands.UpdateExternalInstitution;

public sealed record UpdateExternalInstitutionCommand(
    Guid Id,
    string? Name,
    int? Type,
    string? Address,
    Guid? ParentId
) : IRequest<MessageResponse>;
