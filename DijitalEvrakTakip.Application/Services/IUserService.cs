using DijitalEvrakTakip.Domain.Entities;

namespace DijitalEvrakTakip.Application.Services;

/// <summary>
/// Kullanıcılar yalnızca okunur (vw_Users: ComAuthorizationUser); ekleme / güncelleme kaynak sistemde yapılır.
/// </summary>
public interface IUserService
{
    public IQueryable<User> GetAll();

    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Login için: silinmiş/pasif kullanıcıyı da döndürür, karar handler'da verilir.
    /// userName AD kullanıcı adıdır (sAMAccountName), "HITIT\" öneki olmadan.
    /// </summary>
    Task<User?> GetByUserNameAsync(string userName, CancellationToken cancellationToken);
}
