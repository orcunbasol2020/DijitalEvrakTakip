using DijitalEvrakTakip.Application.Features.UserLoginLogFeatures.Queries.GetUserLoginLogs;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Enums;

namespace DijitalEvrakTakip.Application.Services;

public interface IUserLoginLogService
{
    Task LogAsync(
        Guid? userId,
        string userName,
        bool isSuccess,
        LoginFailureReasonEnum? failureReason,
        string ipAddress,
        string userAgent,
        CancellationToken cancellationToken);

    Task<PagedResultDto<UserLoginLogDto>> GetPagedAsync(
        GetUserLoginLogsQuery request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Verilen kullanıcıların son başarılı girişine göre oturum durumlarını döndürür.
    /// Hiç başarılı girişi olmayan kullanıcı sözlükte yer almaz.
    /// </summary>
    Task<IDictionary<Guid, UserLoginStatusDto>> GetLoginStatusesAsync(
        IReadOnlyCollection<Guid> userIds,
        CancellationToken cancellationToken);
}
