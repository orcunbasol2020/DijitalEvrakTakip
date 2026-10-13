using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Enums;
using MediatR;
using System.Security.Cryptography;
using System.Text;

namespace DijitalEvrakTakip.Application.Features.UserFeatures.Queries.GetUserByUsername;

/// <summary>
/// Kullanıcı adı + ortak geliştirme şifresiyle giriş: yalnızca geliştirme ortamında başka kullanıcılarla
/// test için (LoginSettings.FormLoginEnabled; canlıda kapalı, uç nokta 404 döner). Kaynak sistemde
/// (ComAuthorizationUser) şifre tutulmadığından tüm kullanıcılar için tek şifre (FormLoginPassword)
/// kullanılır. Canlıda kullanıcı Windows oturumuyla girer (WindowsLogin).
/// </summary>
public sealed class GetUserByUsernameHandler : IRequestHandler<GetUserByUsernameQuery, UserLoginDto>
{
    private readonly IUserService _userService;
    private readonly UserLoginFlow _loginFlow;

    public GetUserByUsernameHandler(IUserService userService, UserLoginFlow loginFlow)
    {
        _userService = userService;
        _loginFlow = loginFlow;
    }

    public async Task<UserLoginDto> Handle(
        GetUserByUsernameQuery request,
        CancellationToken cancellationToken)
    {
        // "HITIT\kullanici" da yazılabilir
        string userName = UserLoginFlow.ToUserName(request.UserName);

        User user = string.IsNullOrEmpty(userName)
            ? null
            : await _userService.GetByUserNameAsync(userName, cancellationToken);

        if (user == null)
            return await _loginFlow.FailAsync(
                null, userName ?? request.UserName, LoginFailureReasonEnum.UserNotFound,
                request.IpAddress, request.UserAgent, cancellationToken);

        if (!IsPasswordValid(request.Password, request.ExpectedPassword))
            return await _loginFlow.FailAsync(
                user.Id, userName, LoginFailureReasonEnum.InvalidPassword,
                request.IpAddress, request.UserAgent, cancellationToken);

        return await _loginFlow.CompleteAsync(
            user, userName, request.IpAddress, request.UserAgent, cancellationToken);
    }

    // Şifre ayarlanmamışsa hiçbir şifre kabul edilmez; karşılaştırma sabit sürelidir.
    private static bool IsPasswordValid(string password, string expected) =>
        !string.IsNullOrEmpty(expected)
        && password != null
        && CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(password), Encoding.UTF8.GetBytes(expected));
}
