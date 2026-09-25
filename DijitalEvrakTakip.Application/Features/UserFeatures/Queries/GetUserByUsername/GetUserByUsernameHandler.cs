using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Enums;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.UserFeatures.Queries.GetUserByUsername;

public sealed class GetUserByUsernameHandler : IRequestHandler<GetUserByUsernameQuery, UserLoginDto>
{
    private readonly IUserService _userService;
    private readonly IUserRoleService _roleService;
    private readonly IJwtProvider _jwtProvider;
    private readonly IUserLoginLogService _loginLogService;

    public GetUserByUsernameHandler(
        IUserService userService,
        IUserRoleService userRoleService,
        IJwtProvider jwtProvider,
        IUserLoginLogService loginLogService)
    {
        _userService = userService;
        _roleService = userRoleService;
        _jwtProvider = jwtProvider;
        _loginLogService = loginLogService;
    }

    public async Task<UserLoginDto> Handle(
        GetUserByUsernameQuery request,
        CancellationToken cancellationToken)
    {
        User user = await _userService.GetByUserNameAsync(request.UserName, cancellationToken);

        if (user == null)
            return await FailAsync(null, request, LoginFailureReasonEnum.UserNotFound, cancellationToken);

        // NOT: Şifre karşılaştırması ileride LDAP bind ile değiştirilecek.
        // Aşağıdaki IsDeleted/IsActive kontrolü, loglama ve token üretimi LDAP'tan bağımsızdır.
        if (user.Password != request.Password)
            return await FailAsync(user.Id, request, LoginFailureReasonEnum.InvalidPassword, cancellationToken);

        if (user.IsDeleted)
            return await FailAsync(user.Id, request, LoginFailureReasonEnum.UserDeleted, cancellationToken);

        if (!user.IsActive)
            return await FailAsync(user.Id, request, LoginFailureReasonEnum.UserInactive, cancellationToken);

        IList<string> roles = await _roleService.GetRolesByUserIdAsync(user.Id, cancellationToken);

        UserLoginDto response = new()
        {
            Id = user.Id,
            Name = user.Name,
            Surname = user.Surname,
            UserName = user.UserName,
            Email = user.Email,

            DepartmentId = user.DepartmentId.ToString(),
            DepartmentName = user.Department?.Name,
            DepartmentShortName = user.Department?.ShortName,

            IsDeleted = user.IsDeleted,
            IsActive = user.IsActive,
            Roles = roles
        };

        (string token, DateTime expiration) = _jwtProvider.CreateToken(response);
        response.Token = token;
        response.TokenExpiration = expiration;

        await _loginLogService.LogAsync(
            user.Id, request.UserName, isSuccess: true, failureReason: null,
            request.IpAddress, request.UserAgent, cancellationToken);

        return response;
    }

    private async Task<UserLoginDto> FailAsync(
        Guid? userId,
        GetUserByUsernameQuery request,
        LoginFailureReasonEnum reason,
        CancellationToken cancellationToken)
    {
        await _loginLogService.LogAsync(
            userId, request.UserName, isSuccess: false, reason,
            request.IpAddress, request.UserAgent, cancellationToken);

        return null;
    }
}
