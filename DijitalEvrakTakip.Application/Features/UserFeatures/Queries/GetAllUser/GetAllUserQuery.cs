using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.UserFeatures.Queries.GetAllUser;

public sealed record GetAllUserQuery(Guid? DepartmentId = null, Guid? RoleId = null) : IRequest<IList<UserDto>>;
