using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.AtlasEbysFeatures.Queries.GetAllAtlasZimmetChanges;

public sealed class GetAllAtlasZimmetChangesQueryHandler
    : IRequestHandler<GetAllAtlasZimmetChangesQuery, IList<AtlasZimmetChangeDto>>
{
    private readonly IAtlasEbysService _atlasEbysService;

    public GetAllAtlasZimmetChangesQueryHandler(IAtlasEbysService atlasEbysService)
    {
        _atlasEbysService = atlasEbysService;
    }

    public async Task<IList<AtlasZimmetChangeDto>> Handle(
        GetAllAtlasZimmetChangesQuery request,
        CancellationToken cancellationToken)
    {
        var entities = await _atlasEbysService.GetAllAsync(cancellationToken);

        return entities
            .OrderByDescending(x => x.CreatedDate)
            .Select(x => new AtlasZimmetChangeDto
            {
                Id = x.Id,
                AtlasZimmetId = x.AtlasZimmetId,
                AtlasDocumentNo = x.AtlasDocumentNo,
                QrCode = x.QrCode,
                FromUserSicilNo = x.FromUserSicilNo,
                FromUserName = x.FromUserName,
                FromUnitName = x.FromUnitName,
                ToUserSicilNo = x.ToUserSicilNo,
                ToUserName = x.ToUserName,
                ToUnitName = x.ToUnitName,
                ZimmetTuru = x.ZimmetTuru,
                ZimmetTarihi = x.ZimmetTarihi,
                Description = x.Description,
                Status = x.Status,
                ErrorMessage = x.ErrorMessage,
                ProcessedDate = x.ProcessedDate,
                IncomingDocumentId = x.IncomingDocumentId,
                CreatedDate = x.CreatedDate
            })
            .ToList();
    }
}
