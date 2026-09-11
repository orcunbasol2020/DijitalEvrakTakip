using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace DijitalEvrakTakip.Persistance.Services;

public sealed class LanguageService : ILanguageService
{
    private readonly ILanguageRepository _languageRepository;
    private readonly IUnitOfWork _unitOfWork;

    public LanguageService(
        ILanguageRepository languageRepository,
        IUnitOfWork unitOfWork)
    {
        _languageRepository = languageRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task CreateAsync(
        Language language,
        CancellationToken cancellationToken)
    {
        await _languageRepository.AddAsync(language, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IList<Language>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        return await _languageRepository
            .GetAll()
            .Where(x => !x.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<Language?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _languageRepository
            .GetAll()
            .FirstOrDefaultAsync(
                x => x.Id == id && !x.IsDeleted,
                cancellationToken);
    }

    public async Task UpdateAsync(
        Language language,
        CancellationToken cancellationToken)
    {
        _languageRepository.Update(language);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(
        Language language,
        CancellationToken cancellationToken)
    {
        language.IsDeleted = true;
        _languageRepository.Update(language);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
