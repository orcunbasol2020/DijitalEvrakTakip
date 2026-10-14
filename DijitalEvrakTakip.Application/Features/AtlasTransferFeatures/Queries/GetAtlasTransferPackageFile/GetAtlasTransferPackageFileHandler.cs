using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.AtlasTransferFeatures.Queries.GetAtlasTransferPackageFile;

public sealed class GetAtlasTransferPackageFileHandler
    : IRequestHandler<GetAtlasTransferPackageFileQuery, AtlasTransferPackageFileDto>
{
    private readonly IAtlasTransferService _transferService;

    public GetAtlasTransferPackageFileHandler(IAtlasTransferService transferService)
    {
        _transferService = transferService;
    }

    public Task<AtlasTransferPackageFileDto> Handle(
        GetAtlasTransferPackageFileQuery request,
        CancellationToken cancellationToken)
    {
        return _transferService.GetPackageFileAsync(request.DocumentId, request.PackageId, cancellationToken);
    }
}
