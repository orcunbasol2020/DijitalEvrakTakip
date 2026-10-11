using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Enums;

namespace DijitalEvrakTakip.Application.Features.UserFeatures;

/// <summary>
/// Kimliği doğrulanmış (şifreyle ya da Windows oturumuyla) kullanıcı için ortak giriş adımları:
/// silinmiş / pasif kontrolü, roller, token üretimi ve giriş logu.
/// </summary>
public sealed class UserLoginFlow
{
    private readonly IUserRoleService _roleService;
    private readonly IJwtProvider _jwtProvider;
    private readonly IUserLoginLogService _loginLogService;

    public UserLoginFlow(
        IUserRoleService userRoleService,
        IJwtProvider jwtProvider,
        IUserLoginLogService loginLogService)
    {
        _roleService = userRoleService;
        _jwtProvider = jwtProvider;
        _loginLogService = loginLogService;
    }

    public async Task<UserLoginDto> CompleteAsync(
        User user,
        string userName,
        string ipAddress,
        string userAgent,
        CancellationToken cancellationToken)
    {
        if (user.IsDeleted)
            return await FailAsync(user.Id, userName, LoginFailureReasonEnum.UserDeleted, ipAddress, userAgent, cancellationToken);

        if (!user.IsActive)
            return await FailAsync(user.Id, userName, LoginFailureReasonEnum.UserInactive, ipAddress, userAgent, cancellationToken);

        IList<string> roles = await _roleService.GetRolesByUserIdAsync(user.Id, cancellationToken);

        UserLoginDto response = new()
        {
            Id = user.Id,
            Name = user.Name,
            Surname = user.Surname,
            UserName = user.UserName,
            Email = user.Email,

            DepartmentId = user.DepartmentId,
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
            user.Id, userName, isSuccess: true, failureReason: null,
            ipAddress, userAgent, cancellationToken);

        return response;
    }

    // "DOMAIN\kullanici" (Windows, ComAuthorizationUser.NTLogonName) ve "kullanici@REALM" (Linux Kerberos)
    // -> "kullanici". Sistemdeki kullanıcı adları AD kullanıcı adıdır (sAMAccountName).
    public static string ToUserName(string identityName)
    {
        if (string.IsNullOrWhiteSpace(identityName))
            return null;

        string name = identityName.Trim();

        int backslash = name.LastIndexOf('\\');
        if (backslash >= 0)
            name = name[(backslash + 1)..];

        int at = name.IndexOf('@');
        if (at >= 0)
            name = name[..at];

        return name.Length == 0 ? null : name;
    }

    public async Task<UserLoginDto> FailAsync(
        Guid? userId,
        string userName,
        LoginFailureReasonEnum reason,
        string ipAddress,
        string userAgent,
        CancellationToken cancellationToken)
    {
        await _loginLogService.LogAsync(
            userId, userName, isSuccess: false, reason,
            ipAddress, userAgent, cancellationToken);

        return null;
    }
}
