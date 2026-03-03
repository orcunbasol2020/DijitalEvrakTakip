using DijitalEvrakTakip.Domain.Entities;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Queries.GetAllIncomingDocument;

public sealed record GetAllIncomingDocumentQuery() : IRequest<IList<IncomingDocument>>
{
    /// <summary>
    /// "completed" | "pending" | "error" | "all" | null
    /// </summary>
    public string? Status { get; set; }
    public string? AssignedUserFullName { get; set; }
}
