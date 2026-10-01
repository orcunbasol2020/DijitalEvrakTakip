using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.OutgoingDocumentDistributionFeatures.Queries.GetOutgoingDocumentDistributionsByDocumentId;

public sealed record GetOutgoingDocumentDistributionsByDocumentIdQuery(
    Guid OutgoingDocumentId
) : IRequest<IList<OutgoingDocumentDistributionDto>>;
