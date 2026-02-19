using DijitalEvrakTakip.Domain.Entities;

namespace DijitalEvrakTakip.Application.Services;

public interface IExternalInstitutionService
{
    Task<IList<ExternalInstitution>> GetAllAsync(CancellationToken cancellationToken);

    Task<ExternalInstitution?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
