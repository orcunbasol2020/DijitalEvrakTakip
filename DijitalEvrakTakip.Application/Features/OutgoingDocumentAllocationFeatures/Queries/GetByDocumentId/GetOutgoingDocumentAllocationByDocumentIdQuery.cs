using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.OutgoingDocumentAllocationFeatures.Queries.GetByDocumentId;

public sealed record GetOutgoingDocumentAllocationByDocumentIdQuery(Guid OutgoingDocumentId)
    : IRequest<IList<OutgoingDocumentAllocationDto>>;
