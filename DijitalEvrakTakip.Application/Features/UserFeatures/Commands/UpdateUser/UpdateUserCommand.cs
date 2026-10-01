using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.UserFeatures.Commands.UpdateUser;

public sealed record UpdateUserCommand(
    Guid Id,
    string? Name,
    string? Surname,
    string? Email,
    string? UserName,
    bool? IsActive,
    Guid? DepartmentId
) : IRequest<MessageResponse>;
