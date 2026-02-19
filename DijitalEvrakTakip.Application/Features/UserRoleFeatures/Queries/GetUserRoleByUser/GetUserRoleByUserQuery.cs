using DijitalEvrakTakip.Domain.Entities;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.UserRoleFeatures.Queries.GetUserRoleByUser;

public sealed record GetUserRoleByUserQuery(
    Guid UserId
    ) : IRequest<IList<UserRole>>;

