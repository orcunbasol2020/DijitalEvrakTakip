using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Queries.GetIncomingDocumentDistributions;

public sealed record GetIncomingDocumentDistributionsQuery(
    Guid IncomingDocumentId
) : IRequest<IList<IncomingDocumentDistributionDto>>;
