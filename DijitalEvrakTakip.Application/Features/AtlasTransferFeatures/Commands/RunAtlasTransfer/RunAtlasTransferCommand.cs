using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.AtlasTransferFeatures.Commands.RunAtlasTransfer;

/// <summary>
/// Arka plan servisini beklemeden kuyruktan bir parti evrakı Atlas'a aktarır.
/// Otomatik aktarım ayarı kapalı olsa da çalışır.
/// </summary>
public sealed record RunAtlasTransferCommand() : IRequest<AtlasTransferRunResultDto>;
