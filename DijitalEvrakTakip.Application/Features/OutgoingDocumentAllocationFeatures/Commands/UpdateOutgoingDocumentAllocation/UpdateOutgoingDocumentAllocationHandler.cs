using MediatR;
using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;

namespace DijitalEvrakTakip.Application.Features.OutgoingDocumentAllocationFeatures.Commands.UpdateOutgoingDocumentAllocation;

public sealed class UpdateOutgoingDocumentAllocationCommandHandler
    : IRequestHandler<UpdateOutgoingDocumentAllocationCommand, MessageResponse>
{
    private readonly IOutgoingDocumentAllocationService _allocationService;
    private readonly IOutgoingDocumentService _outgoingDocumentService;

    public UpdateOutgoingDocumentAllocationCommandHandler(
        IOutgoingDocumentAllocationService allocationService,
        IOutgoingDocumentService outgoingDocumentService)
    {
        _allocationService = allocationService;
        _outgoingDocumentService = outgoingDocumentService;
    }

    public async Task<MessageResponse> Handle(
        UpdateOutgoingDocumentAllocationCommand request,
        CancellationToken cancellationToken)
    {
        var allocation = await _allocationService
            .GetByIdAsync(request.Id, cancellationToken);

        if (allocation is null)
            return new MessageResponse("Zimmet kaydı bulunamadı");

        if (request.OutgoingDocumentId.HasValue)
        {
            var outgoingDocument = await _outgoingDocumentService.GetByIdAsync(
                request.OutgoingDocumentId.Value,
                cancellationToken);

            if (outgoingDocument is null)
                return new MessageResponse(
                    "Geçersiz OutgoingDocumentId: bu Id'ye sahip bir evrak bulunamadı. " +
                    "Zarfa evrak eklerken dönen 'DocumentId' alanı gönderilmelidir, 'Id' alanı değil.");

            allocation.OutgoingDocumentId = request.OutgoingDocumentId.Value;
        }

        if (!string.IsNullOrWhiteSpace(request.UserId))
            allocation.UserId = Guid.Parse(request.UserId);

        if (request.UserType.HasValue)
            allocation.UserType = request.UserType.Value;

        if (request.Status.HasValue)
            allocation.Status = request.Status.Value;

        if (request.IsActive.HasValue)
            allocation.IsActive = request.IsActive.Value;

        await _allocationService.UpdateAsync(allocation, cancellationToken);

        return new MessageResponse("Zimmet kaydı güncellendi");
    }
}
