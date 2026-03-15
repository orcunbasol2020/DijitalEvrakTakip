using MediatR;
using DijitalEvrakTakip.Domain.Dtos;

namespace DijitalEvrakTakip.Application.Features.IncomingDocumentFeatures.Queries.GetIncomingDocumentLast30DaysStats;

public sealed record GetIncomingDocumentLast30DaysStatsQuery() : IRequest<IncomingDocumentLast30DaysStatsDto>;