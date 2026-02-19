using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Entities;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.UserRoleFeatures.Queries.GetUserRoleByUser;

public sealed class GetUserByUserHandler : IRequestHandler<GetUserRoleByUserQuery, IList<UserRole>>
{
    private readonly IUserRoleService _userRoleService;

    public GetUserByUserHandler(IUserRoleService userRoleService)
    {
        _userRoleService = userRoleService;
    }

    public async Task<IList<UserRole>> Handle(GetUserRoleByUserQuery request, CancellationToken cancellationToken)
    {
        IList<UserRole> userRoles = await _userRoleService.GetUserRoleByUser(
            x => x.UserId == request.UserId,
            cancellationToken);

        return userRoles;
    }


}

