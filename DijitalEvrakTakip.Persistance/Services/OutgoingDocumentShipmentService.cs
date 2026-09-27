using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Enums;
using DijitalEvrakTakip.Domain.Repositories;
using GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace DijitalEvrakTakip.Persistance.Services;

public sealed class OutgoingDocumentShipmentService : IOutgoingDocumentShipmentService
{
    private readonly IOutgoingDocumentShipmentRepository _shipmentRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public OutgoingDocumentShipmentService(
        IOutgoingDocumentShipmentRepository shipmentRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _shipmentRepository = shipmentRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task CreateAsync(OutgoingDocumentShipment shipment, CancellationToken cancellationToken)
    {
        await _shipmentRepository.AddAsync(shipment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(OutgoingDocumentShipment shipment, CancellationToken cancellationToken)
    {
        _shipmentRepository.Update(shipment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<OutgoingDocumentShipment?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await Query()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<OutgoingDocumentShipment?> GetForUpdateAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _shipmentRepository
            .GetAll()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<OutgoingDocumentShipmentDto?> GetDtoByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var shipment = await GetByIdAsync(id, cancellationToken);
        if (shipment is null)
            return null;

        var names = await GetSenderNamesAsync(new[] { shipment }, cancellationToken);
        return ToDto(shipment, names);
    }

    public async Task<OutgoingDocumentShipmentDto?> GetDtoByTrackingNumberAsync(
        string trackingNumber,
        CancellationToken cancellationToken)
    {
        var shipment = await Query()
            .Where(x => x.TrackingNumber == trackingNumber)
            .OrderByDescending(x => x.CreatedDate)
            .FirstOrDefaultAsync(cancellationToken);

        if (shipment is null)
            return null;

        var names = await GetSenderNamesAsync(new[] { shipment }, cancellationToken);
        return ToDto(shipment, names);
    }

    public async Task<IList<OutgoingDocumentShipmentDto>> GetDtosByOutgoingDocumentIdAsync(
        Guid outgoingDocumentId,
        CancellationToken cancellationToken)
    {
        var shipments = await Query()
            .Where(x => x.Distributions!.Any(d => d.OutgoingDocumentId == outgoingDocumentId && !d.IsDeleted))
            .OrderByDescending(x => x.SentDate)
            .ToListAsync(cancellationToken);

        var names = await GetSenderNamesAsync(shipments, cancellationToken);

        return shipments.Select(x => ToDto(x, names)).ToList();
    }

    // Paket içeriği (dağıtım satırları) evrak, birim ve kurum bilgisiyle birlikte yüklenir
    private IQueryable<OutgoingDocumentShipment> Query()
    {
        return _shipmentRepository
            .GetAll()
            .Include(x => x.ExternalInstitution)
            .Include(x => x.Distributions!.Where(d => !d.IsDeleted))
                .ThenInclude(d => d.OutgoingDocument)
            .Include(x => x.Distributions!.Where(d => !d.IsDeleted))
                .ThenInclude(d => d.Department)
            .Include(x => x.Distributions!.Where(d => !d.IsDeleted))
                .ThenInclude(d => d.ExternalInstitution)
            .Where(x => !x.IsDeleted);
    }

    private async Task<Dictionary<Guid, string>> GetSenderNamesAsync(
        IEnumerable<OutgoingDocumentShipment> shipments,
        CancellationToken cancellationToken)
    {
        var userIds = shipments.Select(x => x.SentUserId).Distinct().ToList();
        if (userIds.Count == 0)
            return new Dictionary<Guid, string>();

        var users = await _userRepository
            .GetAll()
            .Where(x => userIds.Contains(x.Id))
            .Select(x => new { x.Id, x.Name, x.Surname })
            .ToListAsync(cancellationToken);

        return users.ToDictionary(x => x.Id, x => $"{x.Name} {x.Surname}");
    }

    private static OutgoingDocumentShipmentDto ToDto(
        OutgoingDocumentShipment x,
        IReadOnlyDictionary<Guid, string> senderNames)
    {
        var items = (x.Distributions ?? Array.Empty<OutgoingDocumentDistribution>())
            .Select(d => new OutgoingDocumentShipmentItemDto(
                d.Id,
                d.OutgoingDocumentId,
                d.OutgoingDocument?.OriginalDocumentNumber,
                d.OutgoingDocument?.Subject,
                d.DepartmentId,
                d.Department?.Name,
                d.ExternalInstitutionId,
                d.ExternalInstitution?.Name))
            .ToList();

        return new OutgoingDocumentShipmentDto(
            x.Id,
            x.CargoCompany,
            Enum.IsDefined(typeof(CargoCompanyEnum), x.CargoCompany)
                ? ((CargoCompanyEnum)x.CargoCompany).GetDescription()
                : "-",
            x.TrackingNumber,
            x.SentDate,
            x.SentUserId,
            senderNames.TryGetValue(x.SentUserId, out var name) ? name : null,
            x.ExternalInstitutionId,
            x.ExternalInstitution?.Name,
            x.RecipientName,
            x.Status,
            Enum.IsDefined(typeof(ShipmentStatusEnum), x.Status)
                ? ((ShipmentStatusEnum)x.Status).GetDescription()
                : "-",
            x.DeliveredDate,
            x.Cost,
            x.Notes,
            items,
            x.CreatedDate,
            x.UpdateDate);
    }
}
