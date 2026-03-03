using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

public sealed record GetDocumentTransactionsByDocumentIdQuery(Guid DocumentId)
    : IRequest<IList<DocumentTransactionDto>>;
