using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.AtlasTransferFeatures.Queries.GetAtlasTransferPackageFile;

/// <summary>Evrakın EYP paket dosyası; PackageId verilmezse güncel paket.</summary>
public sealed record GetAtlasTransferPackageFileQuery(Guid DocumentId, Guid? PackageId) : IRequest<AtlasTransferPackageFileDto>;
