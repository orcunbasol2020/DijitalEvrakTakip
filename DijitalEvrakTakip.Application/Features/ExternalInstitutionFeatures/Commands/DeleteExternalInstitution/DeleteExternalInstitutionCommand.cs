using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.ExternalInstitutionFeatures.Commands.DeleteExternalInstitution;

public sealed record DeleteExternalInstitutionCommand(
    Guid Id
) : IRequest<MessageResponse>;
