using DijitalEvrakTakip.Domain.Entities;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.OutgoingDocumentFeatures.Queries.GetAllOutgoingDocument;

public sealed record GetAllOutgoingDocumentQuery() : IRequest<IList<OutgoingDocument>>
{
    /// <summary>
    /// OutgoingDocumentStatusEnum değeri. Verilmezse tüm kayıtlar döner.
    /// </summary>
    public int? Status { get; set; }
}
