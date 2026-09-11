using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.UserFeatures.Commands.DeleteUser;

public sealed record DeleteUserCommand(
    Guid Id
) : IRequest<MessageResponse>;
