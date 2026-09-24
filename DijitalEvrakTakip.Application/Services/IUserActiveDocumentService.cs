using DijitalEvrakTakip.Domain.Dtos;

namespace DijitalEvrakTakip.Application.Services;

public interface IUserActiveDocumentService
{
    /// <summary>
    /// Kullanıcı üzerinde aktif zimmetli gelen ve giden evrakları tek listede döner.
    /// </summary>
    /// <param name="userId">Zimmet sahibi kullanıcı.</param>
    /// <param name="documentDirection">DocumentDirectionEnum; null ise gelen + giden birlikte.</param>
    /// <param name="page">1 tabanlı sayfa numarası. pageSize verilmezse yok sayılır.</param>
    /// <param name="pageSize">Sayfa boyutu. null ise tüm kayıtlar döner.</param>
    Task<PagedResultDto<UserActiveDocumentDto>> GetActiveByUserIdAsync(
        Guid userId,
        int? documentDirection,
        int? page,
        int? pageSize,
        CancellationToken cancellationToken);
}
