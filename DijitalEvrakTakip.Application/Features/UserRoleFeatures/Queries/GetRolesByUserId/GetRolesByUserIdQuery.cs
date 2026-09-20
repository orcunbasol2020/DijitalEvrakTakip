using MediatR;

namespace DijitalEvrakTakip.Application.Features.UserRoleFeatures.Queries.GetRolesByUserId;

public sealed record GetRolesByUserIdQuery(
    Guid UserId
    ) : IRequest<IList<string>>;
