using MediatR;
using DijitalEvrakTakip.Domain.Dtos;

namespace DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Queries.GetIncomingDocumentTodayStats;

public sealed record GetIncomingDocumentTodayStatsQuery() : IRequest<IncomingDocumentTodayStatsDto>;