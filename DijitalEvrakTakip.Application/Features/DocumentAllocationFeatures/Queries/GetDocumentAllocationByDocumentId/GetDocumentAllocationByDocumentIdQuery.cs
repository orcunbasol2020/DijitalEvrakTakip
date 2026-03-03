using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.DocumentAllocationFeatures.Queries.GetByDocumentId;

public sealed record GetDocumentAllocationByDocumentIdQuery(Guid IncomingDocumentId)
    : IRequest<IList<DocumentAllocationDto>>;