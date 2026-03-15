using DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Commands.CreateIncomingDocument;
using DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Commands.UpdateIncomingDocument;
using DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Queries.GetAllIncomingDocument;
using DijitalEvrakTakip.Application.Features.ScannedDocumentFeatures.Commands.UpdateScannedDocument;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;

public interface IIncomingDocumentService
{
    Task CreateAsync(CreateIncomingDocumentCommand request, CancellationToken cancellationToken);

    Task UpdateAsync(UpdateIncomingDocumentCommand request, CancellationToken cancellationToken);

    Task<IList<IncomingDocument>> GetAllAsync(GetAllIncomingDocumentQuery request, CancellationToken cancellationToken);

    Task<IncomingDocument?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IncomingDocument?> GetByQrCodeAsync(string qrCode, CancellationToken cancellationToken);

    /// <summary>
    /// Assignment atandığında IncomingDocument tablosundaki CurrentAssignmentUserId alanını set eder.
    /// SaveChanges handler tarafında yapılacak.
    /// </summary>
    Task SetCurrentAssignmentAsync(Guid documentId, Guid? userId);

    Task<int> GetPendingCountByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<IncomingDocumentTodayStatsDto> GetTodayStatsAsync(CancellationToken cancellationToken);
    Task<IncomingDocumentLast30DaysStatsDto> GetLast30DaysStatsAsync(CancellationToken cancellationToken);

}