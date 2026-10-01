using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.ScannedDocumentFeatures.Commands.ImportScannedDocuments;

/// <summary>
/// Tarama klasörünü hemen kontrol eder. Otomatik içe aktarma kapalı olsa da çalışır.
/// </summary>
public sealed record ImportScannedDocumentsCommand() : IRequest<ScannedDocumentImportResultDto>;
