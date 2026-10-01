using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.UserFeatures.Queries.GetUserById;

public sealed record GetUserByIdQuery(
    Guid Id
) : IRequest<UserDto?>;
