using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.AtlasDocumentNumberFeatures.Commands.RefillAtlasDocumentNumberPool;

/// <summary>
/// Havuzu hemen kontrol eder ve stok azsa Atlas'tan doldurur. Havuz ayarı kapalı olsa da çalışır.
/// </summary>
public sealed record RefillAtlasDocumentNumberPoolCommand() : IRequest<AtlasDocumentNumberRefillResultDto>;
