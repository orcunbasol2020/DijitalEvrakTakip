using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Enums;
using DijitalEvrakTakip.Domain.Repositories;
using GenericRepository;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.OutgoingDocumentShipmentFeatures.Commands.CreateOutgoingDocumentShipment;

public sealed class CreateOutgoingDocumentShipmentCommandHandler
    : IRequestHandler<CreateOutgoingDocumentShipmentCommand, OutgoingDocumentShipmentDto>
{
    private readonly IOutgoingDocumentShipmentService _shipmentService;
    private readonly IOutgoingDocumentDistributionService _distributionService;
    private readonly IOutgoingDocumentAllocationService _allocationService;
    private readonly IEnvelopeRepository _envelopeRepository;
    private readonly IOutgoingDocumentTransactionRepository _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOutgoingDocumentShipmentCommandHandler(
        IOutgoingDocumentShipmentService shipmentService,
        IOutgoingDocumentDistributionService distributionService,
        IOutgoingDocumentAllocationService allocationService,
        IEnvelopeRepository envelopeRepository,
        IOutgoingDocumentTransactionRepository transactionRepository,
        IUnitOfWork unitOfWork)
    {
        _shipmentService = shipmentService;
        _distributionService = distributionService;
        _allocationService = allocationService;
        _envelopeRepository = envelopeRepository;
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<OutgoingDocumentShipmentDto> Handle(
        CreateOutgoingDocumentShipmentCommand request,
        CancellationToken cancellationToken)
    {
        var distributions = await _distributionService.GetByIdsAsync(request.DistributionIds, cancellationToken);

        var requestedIds = request.DistributionIds.Distinct().ToList();
        if (distributions.Count != requestedIds.Count)
            throw new Exception("Bir veya daha fazla dağıtım kaydı bulunamadı");

        var alreadyShipped = distributions.Where(x => x.ShipmentId.HasValue).ToList();
        if (alreadyShipped.Count > 0)
            throw new Exception("Seçilen dağıtım kayıtlarından bazıları zaten kargoya verilmiş");

        var documentIds = distributions.Select(x => x.OutgoingDocumentId).Distinct().ToList();

        // Kargoya vermeden önce tüm evrakların zimmetini doğrula; hata varsa hiçbir kayıt oluşmasın
        var activeAllocations = new List<OutgoingDocumentAllocation>();
        foreach (var documentId in documentIds)
        {
            var active = await _allocationService.GetActiveByDocumentIdAsync(documentId, cancellationToken);
            if (active is null)
                continue;

            if (active.UserId != request.SentUserId)
                throw new Exception("Evrak kargoya veren kullanıcıya zimmetli değil. Önce zimmet devri yapılmalıdır.");

            activeAllocations.Add(active);
        }

        // Zarf verildiyse kayıt oluşturmadan önce doğrula
        Envelope? envelope = null;
        if (request.EnvelopeId.HasValue)
        {
            envelope = await _envelopeRepository.GetByExpressionAsync(
                x => x.Id == request.EnvelopeId.Value && !x.IsDeleted, cancellationToken);
            if (envelope is null)
                throw new Exception("Zarf bulunamadı");

            if (envelope.Status == (int)EnvelopeStatusEnum.KargoyaVerildi)
                throw new Exception("Zarf zaten kargoya verilmiş");
        }

        // Alıcı kurum verilmediyse tüm satırlar aynı dış kuruma gidiyorsa oradan al,
        // değilse zarfın kurumunu kullan
        var externalInstitutionId = request.ExternalInstitutionId;
        if (!externalInstitutionId.HasValue)
        {
            var institutionIds = distributions
                .Where(x => x.ExternalInstitutionId.HasValue)
                .Select(x => x.ExternalInstitutionId!.Value)
                .Distinct()
                .ToList();

            if (institutionIds.Count == 1)
                externalInstitutionId = institutionIds[0];
            else if (envelope?.ExternalInstitutionId is not null)
                externalInstitutionId = envelope.ExternalInstitutionId;
        }

        var sentDate = request.SentDate ?? DateTime.UtcNow;

        var shipment = new OutgoingDocumentShipment
        {
            CargoCompany = request.CargoCompany,
            TrackingNumber = request.TrackingNumber.Trim(),
            SentDate = sentDate,
            SentUserId = request.SentUserId,
            ExternalInstitutionId = externalInstitutionId,
            RecipientName = request.RecipientName,
            Status = (int)ShipmentStatusEnum.Shipped,
            Cost = request.Cost,
            Notes = request.Notes
        };

        await _shipmentService.CreateAsync(shipment, cancellationToken);

        foreach (var distribution in distributions)
        {
            distribution.ShipmentId = shipment.Id;
            distribution.DeliveryMethod = (int)DeliveryMethodEnum.Kargo;
            distribution.SentDate = sentDate;
        }

        await _distributionService.UpdateRangeAsync(distributions, cancellationToken);

        // Fiziksel evrak artık kimsede değil: aktif zimmeti kapat
        foreach (var allocation in activeAllocations)
        {
            allocation.IsActive = false;
            allocation.Status = (int)AllocationStatusEnum.KargoyaVerildi;
            await _allocationService.UpdateAsync(allocation, cancellationToken);
        }

        foreach (var documentId in documentIds)
        {
            await _transactionRepository.AddAsync(new OutgoingDocumentTransaction
            {
                OutgoingDocumentId = documentId,
                Type = (int)OutgoingTransactionTypeEnum.Sent,
                ShipmentId = shipment.Id,
                UserId = request.SentUserId.ToString()
            }, cancellationToken);
        }

        // Zarf durumu evrak işlemleriyle aynı SaveChanges içinde güncellenir
        if (envelope is not null)
        {
            envelope.Status = (int)EnvelopeStatusEnum.KargoyaVerildi;
            _envelopeRepository.Update(envelope);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return (await _shipmentService.GetDtoByIdAsync(shipment.Id, cancellationToken))!;
    }
}
