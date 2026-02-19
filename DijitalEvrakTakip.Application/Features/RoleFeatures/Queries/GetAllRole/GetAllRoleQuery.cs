using DijitalEvrakTakip.Domain.Entities;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.RoleFeatures.Queries.GetAllRole;

public sealed record GetAllRoleQuery : IRequest<IList<Role>> { }


