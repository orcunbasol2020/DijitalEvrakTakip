using DijitalEvrakTakip.Domain.Entities;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Queries.GetAllIncomingDocument;

public sealed record GetDocumentsByDirectionQuery() : IRequest<IList<IncomingDocument>>
{
    public string? DocumentDirection { get; set; }
}