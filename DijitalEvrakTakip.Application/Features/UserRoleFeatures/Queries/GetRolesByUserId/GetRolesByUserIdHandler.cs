using DijitalEvrakTakip.Application.Services;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.UserRoleFeatures.Queries.GetRolesByUserId;

public sealed class GetRolesByUserIdHandler : IRequestHandler<GetRolesByUserIdQuery, IList<string>>
{
    private readonly IUserRoleService _userRoleService;

    public GetRolesByUserIdHandler(IUserRoleService userRoleService)
    {
        _userRoleService = userRoleService;
    }

    public async Task<IList<string>> Handle(GetRolesByUserIdQuery request, CancellationToken cancellationToken)
    {
        IList<string> roles = await _userRoleService.GetRolesByUserIdAsync(request.UserId, cancellationToken);
        return roles;
    }
}
