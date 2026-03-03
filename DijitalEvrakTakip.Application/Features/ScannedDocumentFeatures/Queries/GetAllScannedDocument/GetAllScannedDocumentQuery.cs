using DijitalEvrakTakip.Domain.Entities;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.ScannedDocumentFeatures.Queries.GetAllScannedDocument;

public sealed record GetAllScannedDocumentQuery()
    : IRequest<IList<ScannedDocument>>
{
    public string? FileName { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}