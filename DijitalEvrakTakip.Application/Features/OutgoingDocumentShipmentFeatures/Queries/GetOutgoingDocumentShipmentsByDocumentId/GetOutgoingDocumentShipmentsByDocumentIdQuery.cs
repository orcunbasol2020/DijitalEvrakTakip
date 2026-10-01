using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.OutgoingDocumentShipmentFeatures.Queries.GetOutgoingDocumentShipmentsByDocumentId;

public sealed record GetOutgoingDocumentShipmentsByDocumentIdQuery(
    Guid OutgoingDocumentId
) : IRequest<IList<OutgoingDocumentShipmentDto>>;
