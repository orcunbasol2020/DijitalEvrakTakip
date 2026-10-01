using DijitalEvrakTakip.Domain.Dtos;

namespace DijitalEvrakTakip.Application.Services;

public interface IJwtProvider
{
    /// <summary>
    /// Kimliği doğrulanmış kullanıcı için imzalı bir JWT üretir.
    /// Kimlik doğrulamanın nasıl yapıldığından (DB, LDAP vb.) bağımsızdır.
    /// </summary>
    (string Token, DateTime Expiration) CreateToken(UserLoginDto user);
}
