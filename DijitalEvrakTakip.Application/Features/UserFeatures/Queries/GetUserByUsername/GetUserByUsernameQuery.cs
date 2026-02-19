using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.UserFeatures.Queries.GetUserByUsername;

public sealed record GetUserByUsernameQuery(
    string UserName,
    string Password
    ) : IRequest<UserLoginDto>;
