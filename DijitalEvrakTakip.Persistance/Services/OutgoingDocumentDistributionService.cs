using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Repositories;
using GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace DijitalEvrakTakip.Persistance.Services;

public sealed class OutgoingDocumentDistributionService : IOutgoingDocumentDistributionService
{
    private readonly IOutgoingDocumentDistributionRepository _distributionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public OutgoingDocumentDistributionService(
        IOutgoingDocumentDistributionRepository distributionRepository,
        IUnitOfWork unitOfWork)
    {
        _distributionRepository = distributionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task CreateRangeAsync(
        IList<OutgoingDocumentDistribution> distributions,
        CancellationToken cancellationToken)
    {
        await _distributionRepository.AddRangeAsync(distributions, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(
        OutgoingDocumentDistribution distribution,
        CancellationToken cancellationToken)
    {
        _distributionRepository.Update(distribution);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<OutgoingDocumentDistribution?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _distributionRepository
            .GetAll()
            .Include(x => x.Department)
            .Include(x => x.ExternalInstitution)
            .Include(x => x.Shipment)
            .Where(x => !x.IsDeleted && x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IList<OutgoingDocumentDistribution>> GetByOutgoingDocumentIdAsync(
        Guid outgoingDocumentId,
        CancellationToken cancellationToken)
    {
        return await _distributionRepository
            .GetAll()
            .Include(x => x.Department)
            .Include(x => x.ExternalInstitution)
            .Include(x => x.Shipment)
            .Where(x => !x.IsDeleted && x.OutgoingDocumentId == outgoingDocumentId)
            .OrderBy(x => x.CreatedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IList<OutgoingDocumentDistribution>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken)
    {
        var idList = ids.Distinct().ToList();

        // Güncelleme amaçlı okunur: navigation yüklenmez, aksi halde Update()
        // birim / kurum kayıtlarını da değişmiş olarak işaretler.
        return await _distributionRepository
            .GetAll()
            .Where(x => !x.IsDeleted && idList.Contains(x.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<IList<OutgoingDocumentDistribution>> GetByShipmentIdAsync(
        Guid shipmentId,
        CancellationToken cancellationToken)
    {
        return await _distributionRepository
            .GetAll()
            .Where(x => !x.IsDeleted && x.ShipmentId == shipmentId)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateRangeAsync(
        IList<OutgoingDocumentDistribution> distributions,
        CancellationToken cancellationToken)
    {
        foreach (var distribution in distributions)
            _distributionRepository.Update(distribution);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAsync(Guid id, CancellationToken cancellationToken)
    {
        var distribution = await _distributionRepository
            .GetAll()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

        if (distribution is null)
            throw new Exception("Dağıtım kaydı bulunamadı");

        distribution.IsDeleted = true;

        _distributionRepository.Update(distribution);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
