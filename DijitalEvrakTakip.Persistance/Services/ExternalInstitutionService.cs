using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Constants;
using DijitalEvrakTakip.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DijitalEvrakTakip.Persistance.Services;

public sealed class ExternalInstitutionService : IExternalInstitutionService
{
    private readonly IExternalInstitutionRepository _repository;

    public ExternalInstitutionService(IExternalInstitutionRepository repository)
    {
        _repository = repository;
    }

    public async Task<IList<ExternalInstitution>> GetAllAsync(
        int? type,
        string? search,
        int? take,
        CancellationToken cancellationToken)
    {
        IQueryable<ExternalInstitution> query = _repository
            .GetAll()
            .AsNoTracking()
            .Where(x => !x.IsDeleted);

        if (type.HasValue)
            query = query.Where(x => x.Type == type.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            string term = search.Trim();
            // Türkçe kuralla: "valiliği" = "VALİLİĞİ"
            query = query.Where(x =>
                EF.Functions.Collate(x.Name, SqlCollations.Turkish).Contains(term) || x.DetsisCode == term);
        }

        query = query.OrderBy(x => x.Name);

        if (take.HasValue)
            query = query.Take(take.Value);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyDictionary<int, string>> GetNamesAsync(
        IEnumerable<int> ids,
        CancellationToken cancellationToken)
    {
        var idList = ids.Distinct().ToList();
        if (idList.Count == 0)
            return new Dictionary<int, string>();

        return await _repository
            .GetAll()
            .AsNoTracking()
            .Where(x => idList.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, x => x.Name, cancellationToken);
    }

    public async Task<ExternalInstitution?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        return await _repository
            .GetAll()
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
