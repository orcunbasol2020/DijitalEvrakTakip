using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using DijitalEvrakTakip.Persistance.Repositories;
using GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace DijitalEvrakTakip.Persistance.Services;

public sealed class EnvelopeService : IEnvelopeService
{
    private readonly IEnvelopeRepository _envelopeRepository;
    private readonly IExternalInstitutionRepository _externalInstitutionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EnvelopeService(
        IEnvelopeRepository envelopeRepository,
        IExternalInstitutionRepository externalInstitutionRepository,
        IUnitOfWork unitOfWork)
    {
        _envelopeRepository = envelopeRepository;
        _externalInstitutionRepository = externalInstitutionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task CreateAsync(
        Envelope envelope,
        CancellationToken cancellationToken)
    {
        await _envelopeRepository.AddAsync(envelope, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(
        Envelope envelope,
        CancellationToken cancellationToken)
    {
        _envelopeRepository.Update(envelope);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IList<Envelope>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        return await _envelopeRepository
            .GetAll()
            .Include(x => x.EnvelopeDocuments!.Where(d => !d.IsDeleted))
            .Where(x => !x.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<Envelope?> GetByNoAsync(
    string envelopeNo,
    CancellationToken cancellationToken)
    {
        return await _envelopeRepository
            .GetAll()
            .Where(x => !x.IsDeleted && x.EnvelopeNo == envelopeNo)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<string> GenerateEnvelopeNoAsync(CancellationToken cancellationToken)
    {
        var lastEnvelope = await _envelopeRepository
            .GetAll()
            .OrderByDescending(x => x.CreatedDate)
            .FirstOrDefaultAsync(cancellationToken);

        int nextNumber = 1;
        if (lastEnvelope != null)
        {
            var lastNo = lastEnvelope.EnvelopeNo?.Split('-').Last();
            if (int.TryParse(lastNo, out int n))
                nextNumber = n + 1;
        }

        return $"ZRF-{DateTime.Now.Year}-{nextNumber:D6}";
    }

    public async Task<ExternalInstitution?> GetExternalInstitutionByIdAsync(
    Guid id, CancellationToken cancellationToken)
    {
        return await _externalInstitutionRepository
            .GetAll()
            .Where(x => !x.IsDeleted && x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Envelope?> GetByIdAsync(
    Guid id,
    CancellationToken cancellationToken)
    {
        return await _envelopeRepository
            .GetAll()
            .Include(x => x.EnvelopeDocuments!.Where(d => !d.IsDeleted))
            .Where(x => !x.IsDeleted && x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }
}