using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.AtlasTransferFeatures.Queries.GetAtlasTransferDocument;

/// <summary>Evrakın Atlas aktarım durumu, EYP paketleri ve gönderim denemeleri.</summary>
public sealed record GetAtlasTransferDocumentQuery(Guid DocumentId) : IRequest<AtlasTransferDocumentDto>;
