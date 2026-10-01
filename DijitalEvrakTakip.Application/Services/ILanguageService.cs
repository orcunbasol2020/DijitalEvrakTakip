using DijitalEvrakTakip.Domain.Entities;

namespace DijitalEvrakTakip.Application.Services;

public interface ILanguageService
{
    Task CreateAsync(Language language, CancellationToken cancellationToken);

    Task<IList<Language>> GetAllAsync(CancellationToken cancellationToken);

    Task<Language?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task UpdateAsync(Language language, CancellationToken cancellationToken);

    Task DeleteAsync(Language language, CancellationToken cancellationToken);
}
