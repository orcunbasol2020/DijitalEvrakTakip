using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.OutgoingDocumentAllocationFeatures.Commands.UploadWetSignedDocument;

public sealed class UploadWetSignedDocumentCommandHandler
    : IRequestHandler<UploadWetSignedDocumentCommand, MessageResponse>
{
    // OutgoingDocumentAllocationDto.IsAllocated (Status == 2) ile aynı anlamı taşır
    private const int AllocatedStatus = 2;

    private readonly IOutgoingDocumentAllocationService _allocationService;
    private readonly IWetSignedDocumentStorageService _storageService;

    public UploadWetSignedDocumentCommandHandler(
        IOutgoingDocumentAllocationService allocationService,
        IWetSignedDocumentStorageService storageService)
    {
        _allocationService = allocationService;
        _storageService = storageService;
    }

    public async Task<MessageResponse> Handle(
        UploadWetSignedDocumentCommand request,
        CancellationToken cancellationToken)
    {
        var allocation = await _allocationService.GetActiveByDocumentIdAsync(
            request.OutgoingDocumentId,
            cancellationToken);

        if (allocation is null)
            return new MessageResponse(
                "Bu evrak için aktif bir zimmet kaydı bulunamadı. " +
                "Islak imzalı belge yalnızca zimmetlenmiş evraklar için yüklenebilir.");

        if (allocation.Status != AllocatedStatus)
            return new MessageResponse(
                "Evrak henüz zimmetlenmedi. Islak imzalı belge yalnızca zimmet tamamlandıktan sonra yüklenebilir.");

        var previousFilePath = allocation.WetSignedDocumentPath;

        await using var contentStream = new MemoryStream(request.FileContent);

        var relativePath = await _storageService.SaveAsync(
            allocation.OutgoingDocumentId,
            request.FileName,
            contentStream,
            cancellationToken);

        allocation.WetSignedDocumentPath = relativePath;
        allocation.WetSignedDocumentFileName = request.FileName;
        allocation.WetSignedDocumentUploadDate = DateTime.Now;
        allocation.WetSignedDocumentUploadedUserId =
            Guid.TryParse(request.UploadedUserId, out var uploadedUserId) ? uploadedUserId : null;

        await _allocationService.UpdateAsync(allocation, cancellationToken);

        if (!string.IsNullOrEmpty(previousFilePath))
            _storageService.Delete(previousFilePath);

        return new MessageResponse("Islak imzalı belge başarıyla yüklendi");
    }
}
