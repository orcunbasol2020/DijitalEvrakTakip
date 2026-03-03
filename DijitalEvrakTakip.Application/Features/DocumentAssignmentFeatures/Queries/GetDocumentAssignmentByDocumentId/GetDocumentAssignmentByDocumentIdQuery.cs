
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.DocumentAssignmentFeatures.Queries.GetByDocumentId;

public sealed record GetDocumentAssignmentByDocumentIdQuery(Guid DocumentId)
    : IRequest<IList<DocumentAssignmentDto>>;