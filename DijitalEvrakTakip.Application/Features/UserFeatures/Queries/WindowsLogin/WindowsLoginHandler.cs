using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Enums;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.UserFeatures.Queries.WindowsLogin;

public sealed class WindowsLoginHandler : IRequestHandler<WindowsLoginQuery, UserLoginDto>
{
    private readonly IUserService _userService;
    private readonly UserLoginFlow _loginFlow;

    public WindowsLoginHandler(IUserService userService, UserLoginFlow loginFlow)
    {
        _userService = userService;
        _loginFlow = loginFlow;
    }

    public async Task<UserLoginDto> Handle(
        WindowsLoginQuery request,
        CancellationToken cancellationToken)
    {
        string userName = UserLoginFlow.ToUserName(request.IdentityName);

        User user = string.IsNullOrEmpty(userName)
            ? null
            : await _userService.GetByUserNameAsync(userName, cancellationToken);

        if (user == null)
            return await _loginFlow.FailAsync(
                null, userName ?? request.IdentityName, LoginFailureReasonEnum.UserNotFound,
                request.IpAddress, request.UserAgent, cancellationToken);

        return await _loginFlow.CompleteAsync(
            user, userName, request.IpAddress, request.UserAgent, cancellationToken);
    }
}
