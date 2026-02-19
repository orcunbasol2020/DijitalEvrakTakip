using DijitalEvrakTakip.Domain.Entities;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.DocumentFeatures.Queries.GetAllDocument;

public sealed record GetAllDocumentQuery() : IRequest<IList<Document>>
{
    /// <summary>
    /// "completed" | "pending" | "error" | "all" | null
    /// </summary>
    public string? OcrStatus { get; set; }


}
