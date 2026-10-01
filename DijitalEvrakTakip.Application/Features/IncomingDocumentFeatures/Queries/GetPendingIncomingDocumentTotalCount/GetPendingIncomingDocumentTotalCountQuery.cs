using MediatR;
using DijitalEvrakTakip.Domain.Dtos;

namespace DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Queries.GetPendingIncomingDocumentCount;

public sealed record GetPendingIncomingDocumentTotalCountQuery()
    : IRequest<IncomingDocumentPendingScanStatsDto>;