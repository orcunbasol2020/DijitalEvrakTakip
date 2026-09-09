using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.ExternalUserFeatures.Commands.DeleteExternalUser;

public sealed record DeleteExternalUserCommand(
    Guid Id
) : IRequest<MessageResponse>;
