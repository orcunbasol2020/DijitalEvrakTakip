using DijitalEvrakTakip.Domain.Entities;

namespace DijitalEvrakTakip.Application.Services;

public interface IEnvelopeService
{
    Task CreateAsync(Envelope envelope, CancellationToken cancellationToken);
    Task UpdateAsync(Envelope envelope, CancellationToken cancellationToken);
    Task<IList<Envelope>> GetAllAsync(CancellationToken cancellationToken);
    Task<string> GenerateEnvelopeNoAsync(CancellationToken cancellationToken);
    Task<Envelope?> GetByNoAsync(string envelopeNo, CancellationToken cancellationToken);
    Task<ExternalInstitution?> GetExternalInstitutionByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Envelope?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}