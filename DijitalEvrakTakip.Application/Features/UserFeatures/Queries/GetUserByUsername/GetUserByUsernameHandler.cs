using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.UserFeatures.Queries.GetUserByUsername;

public sealed class GetUserByUsernameHandler : IRequestHandler<GetUserByUsernameQuery, UserLoginDto>
{
    private readonly IUserService _userService;
    private readonly IUserRoleService _roleService;

    public GetUserByUsernameHandler(IUserService userService, IUserRoleService userRoleService)
    {
        _userService = userService;
        _roleService = userRoleService;
    }

    public async Task<UserLoginDto> Handle(
    GetUserByUsernameQuery request,
    CancellationToken cancellationToken)
    {
        var user = await _userService.GetUserByUserName(
            x => x.UserName == request.UserName &&
                 x.Password == request.Password,
            cancellationToken);

        if (user == null)
            return null;

        IList<string> roles = await _roleService.GetRolesByUserIdAsync(
            user.Id, cancellationToken);

        return new UserLoginDto
        {
            Id = user.Id,
            Name = user.Name,
            Surname = user.Surname,
            UserName = user.UserName,
            Email = user.Email,

            DepartmentId = user.DepartmentId,
            DepartmentName = user.DepartmentName,
            DepartmentShortName = user.DepartmentShortName,

            IsDeleted = user.IsDeleted,
            IsActive = user.IsActive,
            Roles = roles
        };
    }


    //public async Task<UserLoginDto> Handle(
    //GetUserByUsernameQuery request,
    //CancellationToken cancellationToken)
    //{
    //    var user = await _userService.GetUserByUserName(
    //        x => x.UserName == request.UserName &&
    //             x.Password == request.Password,
    //        cancellationToken);

    //    if (user == null)
    //        return null;

    //    // ROLLERİ ÇEK
    //    IList<string> roles = await _roleService.GetRolesByUserIdAsync(
    //        user.Id.ToString(), cancellationToken);

    //    return new UserLoginDto
    //    {
    //        Id = user.Id.ToString(),
    //        Name = user.Name,
    //        Surname = user.Surname,
    //        UserName = user.UserName,
    //        Email = user.Email,
    //        IsDeleted = user.IsDeleted,
    //        IsActive = user.IsActive,
    //        Roles = roles
    //    };
    //}



}
