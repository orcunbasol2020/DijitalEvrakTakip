using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.OutgoingDocumentAllocationFeatures.Queries.GetActiveByDocumentId;

public sealed record GetActiveOutgoingDocumentAllocationByDocumentIdQuery(Guid OutgoingDocumentId)
    : IRequest<OutgoingDocumentAllocationDto>;
