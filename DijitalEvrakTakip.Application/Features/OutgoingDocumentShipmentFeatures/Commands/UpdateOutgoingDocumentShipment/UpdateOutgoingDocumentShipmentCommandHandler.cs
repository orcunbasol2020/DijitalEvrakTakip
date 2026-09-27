using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Enums;
using DijitalEvrakTakip.Domain.Repositories;
using GenericRepository;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.OutgoingDocumentShipmentFeatures.Commands.UpdateOutgoingDocumentShipment;

public sealed class UpdateOutgoingDocumentShipmentCommandHandler
    : IRequestHandler<UpdateOutgoingDocumentShipmentCommand, OutgoingDocumentShipmentDto>
{
    private readonly IOutgoingDocumentShipmentService _shipmentService;
    private readonly IOutgoingDocumentDistributionService _distributionService;
    private readonly IOutgoingDocumentTransactionRepository _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateOutgoingDocumentShipmentCommandHandler(
        IOutgoingDocumentShipmentService shipmentService,
        IOutgoingDocumentDistributionService distributionService,
        IOutgoingDocumentTransactionRepository transactionRepository,
        IUnitOfWork unitOfWork)
    {
        _shipmentService = shipmentService;
        _distributionService = distributionService;
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<OutgoingDocumentShipmentDto> Handle(
        UpdateOutgoingDocumentShipmentCommand request,
        CancellationToken cancellationToken)
    {
        var shipment = await _shipmentService.GetForUpdateAsync(request.Id, cancellationToken);
        if (shipment is null)
            throw new Exception("Kargo kaydı bulunamadı");

        if (request.CargoCompany.HasValue)
        {
            if (!Enum.IsDefined(typeof(CargoCompanyEnum), request.CargoCompany.Value))
                throw new Exception("Geçersiz kargo firması");

            shipment.CargoCompany = request.CargoCompany.Value;
        }

        if (!string.IsNullOrWhiteSpace(request.TrackingNumber))
            shipment.TrackingNumber = request.TrackingNumber.Trim();

        if (request.RecipientName != null) shipment.RecipientName = request.RecipientName;
        if (request.Cost.HasValue) shipment.Cost = request.Cost;
        if (request.Notes != null) shipment.Notes = request.Notes;
        if (request.DeliveredDate.HasValue) shipment.DeliveredDate = request.DeliveredDate;

        var statusChanged = false;
        if (request.Status.HasValue && request.Status.Value != shipment.Status)
        {
            if (!Enum.IsDefined(typeof(ShipmentStatusEnum), request.Status.Value))
                throw new Exception("Geçersiz kargo durumu: 1 (Kargoya Verildi), 2 (Yolda), 3 (Teslim Edildi) veya 4 (İade) olmalıdır.");

            shipment.Status = request.Status.Value;
            statusChanged = true;
        }

        var distributions = await _distributionService.GetByShipmentIdAsync(shipment.Id, cancellationToken);

        if (statusChanged && shipment.Status == (int)ShipmentStatusEnum.Delivered)
        {
            shipment.DeliveredDate ??= DateTime.UtcNow;

            foreach (var distribution in distributions)
                distribution.DeliveryDate ??= shipment.DeliveredDate;
        }

        await _shipmentService.UpdateAsync(shipment, cancellationToken);

        if (statusChanged && shipment.Status == (int)ShipmentStatusEnum.Delivered && distributions.Count > 0)
            await _distributionService.UpdateRangeAsync(distributions, cancellationToken);

        // Teslim / iade durumlarını evrak işlem geçmişine de yaz
        if (statusChanged &&
            (shipment.Status == (int)ShipmentStatusEnum.Delivered || shipment.Status == (int)ShipmentStatusEnum.Returned))
        {
            var transactionType = shipment.Status == (int)ShipmentStatusEnum.Delivered
                ? OutgoingTransactionTypeEnum.Delivered
                : OutgoingTransactionTypeEnum.Returned;

            foreach (var documentId in distributions.Select(x => x.OutgoingDocumentId).Distinct())
            {
                await _transactionRepository.AddAsync(new OutgoingDocumentTransaction
                {
                    OutgoingDocumentId = documentId,
                    Type = (int)transactionType,
                    ShipmentId = shipment.Id,
                    UserId = request.UpdatedUserId?.ToString()
                }, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return (await _shipmentService.GetDtoByIdAsync(shipment.Id, cancellationToken))!;
    }
}
