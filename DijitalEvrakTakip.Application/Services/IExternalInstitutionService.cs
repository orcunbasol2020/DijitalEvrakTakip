using DijitalEvrakTakip.Domain.Entities;

namespace DijitalEvrakTakip.Application.Services;

public interface IExternalInstitutionService
{
    Task CreateAsync(ExternalInstitution institution, CancellationToken cancellationToken);

    Task<IList<ExternalInstitution>> GetAllAsync(CancellationToken cancellationToken);

    Task<ExternalInstitution?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task UpdateAsync(ExternalInstitution institution, CancellationToken cancellationToken);

    Task DeleteAsync(ExternalInstitution institution, CancellationToken cancellationToken);
}
