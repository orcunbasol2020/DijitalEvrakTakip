using MediatR;
using DijitalEvrakTakip.Domain.Dtos;

namespace DijitalEvrakTakip.Application.Features.OutgoingDocumentAllocationFeatures.Queries.GetWetSignedDocument
{
    public sealed record GetWetSignedDocumentQuery(
        Guid AllocationId
    ) : IRequest<WetSignedDocumentFileDto?>;
}
