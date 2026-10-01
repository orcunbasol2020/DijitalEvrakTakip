using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.OutgoingDocumentShipmentFeatures.Queries.GetOutgoingDocumentShipmentById;

public sealed record GetOutgoingDocumentShipmentByIdQuery(
    Guid Id
) : IRequest<OutgoingDocumentShipmentDto?>;
