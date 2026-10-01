using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.OutgoingDocumentFeatures.Queries.GetOutgoingDocumentTransactionsByDocumentId;

public sealed record GetOutgoingDocumentTransactionsByDocumentIdQuery(Guid OutgoingDocumentId)
    : IRequest<IList<OutgoingDocumentTransactionDto>>;
