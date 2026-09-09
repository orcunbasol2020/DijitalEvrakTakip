using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace DijitalEvrakTakip.Persistance.Services;

public sealed class ExternalInstitutionService : IExternalInstitutionService
{
    private readonly IExternalInstitutionRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public ExternalInstitutionService(
        IExternalInstitutionRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    // Yeni kurum ekleme
    public async Task CreateAsync(
        ExternalInstitution institution,
        CancellationToken cancellationToken)
    {
        await _repository.AddAsync(institution, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    // Tüm kurumları getirme (dropdown için)
    public async Task<IList<ExternalInstitution>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        return await _repository
            .GetAll()
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<ExternalInstitution?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _repository
            .GetAll()
            .Where(x => !x.IsDeleted && x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
