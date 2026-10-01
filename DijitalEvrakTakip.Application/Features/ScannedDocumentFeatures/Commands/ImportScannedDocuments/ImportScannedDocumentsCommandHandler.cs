using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.ScannedDocumentFeatures.Commands.ImportScannedDocuments;

public sealed class ImportScannedDocumentsCommandHandler
    : IRequestHandler<ImportScannedDocumentsCommand, ScannedDocumentImportResultDto>
{
    private readonly IScannedDocumentImportService _importService;

    public ImportScannedDocumentsCommandHandler(IScannedDocumentImportService importService)
    {
        _importService = importService;
    }

    public Task<ScannedDocumentImportResultDto> Handle(
        ImportScannedDocumentsCommand request,
        CancellationToken cancellationToken)
    {
        return _importService.ImportAsync(ignoreEnabledFlag: true, cancellationToken);
    }
}
