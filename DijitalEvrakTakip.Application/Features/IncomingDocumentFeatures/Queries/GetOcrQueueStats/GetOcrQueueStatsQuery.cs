using MediatR;
using DijitalEvrakTakip.Domain.Dtos;

namespace DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Queries.GetOcrQueueStats;

public sealed record GetOcrQueueStatsQuery()
    : IRequest<IncomingDocumentOcrQueueStatsDto>;