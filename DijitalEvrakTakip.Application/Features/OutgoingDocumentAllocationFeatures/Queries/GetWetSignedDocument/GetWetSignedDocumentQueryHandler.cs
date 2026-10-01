using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.OutgoingDocumentAllocationFeatures.Queries.GetWetSignedDocument;

public sealed class GetWetSignedDocumentQueryHandler
    : IRequestHandler<GetWetSignedDocumentQuery, WetSignedDocumentFileDto?>
{
    private readonly IOutgoingDocumentAllocationService _allocationService;
    private readonly IWetSignedDocumentStorageService _storageService;

    public GetWetSignedDocumentQueryHandler(
        IOutgoingDocumentAllocationService allocationService,
        IWetSignedDocumentStorageService storageService)
    {
        _allocationService = allocationService;
        _storageService = storageService;
    }

    public async Task<WetSignedDocumentFileDto?> Handle(
        GetWetSignedDocumentQuery request,
        CancellationToken cancellationToken)
    {
        var allocation = await _allocationService.GetByIdAsync(request.AllocationId, cancellationToken);

        if (allocation is null || string.IsNullOrEmpty(allocation.WetSignedDocumentPath))
            return null;

        var file = await _storageService.ReadAsync(allocation.WetSignedDocumentPath, cancellationToken);

        if (file is null)
            return null;

        return new WetSignedDocumentFileDto
        {
            Content = file.Value.Content,
            ContentType = file.Value.ContentType,
            FileName = allocation.WetSignedDocumentFileName ?? "belge"
        };
    }
}
