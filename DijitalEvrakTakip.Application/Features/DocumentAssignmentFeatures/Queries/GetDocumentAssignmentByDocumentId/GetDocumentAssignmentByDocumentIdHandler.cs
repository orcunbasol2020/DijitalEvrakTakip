using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.DocumentAssignmentFeatures.Queries.GetByDocumentId;

public sealed class GetDocumentAssignmentByDocumentIdHandler
    : IRequestHandler<GetDocumentAssignmentByDocumentIdQuery, IList<DocumentAssignmentDto>>
{
    private readonly IDocumentAssignmentService _documentAssignmentService;

    public GetDocumentAssignmentByDocumentIdHandler(
        IDocumentAssignmentService documentAssignmentService)
    {
        _documentAssignmentService = documentAssignmentService;
    }

    public async Task<IList<DocumentAssignmentDto>> Handle(
        GetDocumentAssignmentByDocumentIdQuery request,
        CancellationToken cancellationToken)
    {
        var assignments = await _documentAssignmentService
            .GetByDocumentIdAsync(request.DocumentId, cancellationToken);

        return assignments;
    }
}