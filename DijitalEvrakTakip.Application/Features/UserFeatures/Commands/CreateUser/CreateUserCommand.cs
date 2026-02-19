using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.UserFeatures.Commands.CreateUser;

public sealed record CreateUserCommand(
    string Name,
    string Surname,
    string Email,
    string UserName,
    bool IsActive,
    string DepartmentId
    ): IRequest<MessageResponse>;
