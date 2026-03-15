using MediatR;

namespace DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Queries.GetPendingIncomingDocumentCount;

public sealed record GetPendingIncomingDocumentCountQuery(Guid UserId) : IRequest<int>;