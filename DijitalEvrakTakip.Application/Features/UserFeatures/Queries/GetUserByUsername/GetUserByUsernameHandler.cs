using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Enums;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.UserFeatures.Queries.GetUserByUsername;

/// <summary>
/// Kullanıcı adıyla ŞİFRESİZ giriş: yalnızca geliştirme ortamında başka kullanıcılarla test için
/// (LoginSettings.FormLoginEnabled; canlıda kapalı, uç nokta 404 döner). Kaynak sistemde
/// (ComAuthorizationUser) şifre tutulmaz; canlıda kullanıcı Windows oturumuyla girer (WindowsLogin).
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

        return await _loginFlow.CompleteAsync(
            user, userName, request.IpAddress, request.UserAgent, cancellationToken);
    }
}
