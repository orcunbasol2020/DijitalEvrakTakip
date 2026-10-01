using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.UserLoginLogFeatures.Queries.GetUserLoginLogs;

public sealed class GetUserLoginLogsHandler
    : IRequestHandler<GetUserLoginLogsQuery, PagedResultDto<UserLoginLogDto>>
{
    private readonly IUserLoginLogService _loginLogService;

    public GetUserLoginLogsHandler(IUserLoginLogService loginLogService)
    {
        _loginLogService = loginLogService;
    }

    public Task<PagedResultDto<UserLoginLogDto>> Handle(
        GetUserLoginLogsQuery request,
        CancellationToken cancellationToken)
    {
        return _loginLogService.GetPagedAsync(request, cancellationToken);
    }
}
