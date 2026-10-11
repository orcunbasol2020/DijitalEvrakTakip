using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Enums;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.UserFeatures.Queries.GetUserByUsername;

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
        User user = await _userService.GetByUserNameAsync(request.UserName, cancellationToken);

        if (user == null)
            return await _loginFlow.FailAsync(
                null, request.UserName, LoginFailureReasonEnum.UserNotFound,
                request.IpAddress, request.UserAgent, cancellationToken);

        // Şifre veritabanındakiyle karşılaştırılır. Bu giriş yalnızca LoginSettings.FormLoginEnabled açıkken
        // (geliştirme ortamı) kullanılır; canlıda kullanıcı Windows oturumuyla girer (WindowsLogin).
        if (user.Password != request.Password)
            return await _loginFlow.FailAsync(
                user.Id, request.UserName, LoginFailureReasonEnum.InvalidPassword,
                request.IpAddress, request.UserAgent, cancellationToken);

        return await _loginFlow.CompleteAsync(
            user, request.UserName, request.IpAddress, request.UserAgent, cancellationToken);
    }
}
