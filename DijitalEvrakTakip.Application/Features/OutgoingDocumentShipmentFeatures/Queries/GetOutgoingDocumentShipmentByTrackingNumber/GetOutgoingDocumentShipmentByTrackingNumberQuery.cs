using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.OutgoingDocumentShipmentFeatures.Queries.GetOutgoingDocumentShipmentByTrackingNumber;

public sealed record GetOutgoingDocumentShipmentByTrackingNumberQuery(
    string TrackingNumber
) : IRequest<OutgoingDocumentShipmentDto?>;
