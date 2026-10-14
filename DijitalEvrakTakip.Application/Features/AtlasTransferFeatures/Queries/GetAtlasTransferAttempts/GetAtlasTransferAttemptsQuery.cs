using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.AtlasTransferFeatures.Queries.GetAtlasTransferAttempts;

/// <summary>Tüm evraklardaki son paket gönderim denemeleri, en yenisi önce (en fazla 500).</summary>
public sealed record GetAtlasTransferAttemptsQuery(int Limit = 50) : IRequest<IReadOnlyList<AtlasTransferAttemptDto>>;
