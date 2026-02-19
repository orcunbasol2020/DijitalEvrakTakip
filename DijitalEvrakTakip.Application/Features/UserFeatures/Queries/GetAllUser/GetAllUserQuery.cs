using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.UserFeatures.Queries.GetAllUser;

public sealed record GetAllUserQuery() : IRequest<IList<UserDto>>;
