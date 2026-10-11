using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Constants;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.ExternalInstitutionFeatures.Queries.GetAllExternalInstitution;

public sealed class GetAllExternalInstitutionQueryHandler
    : IRequestHandler<GetAllExternalInstitutionQuery, IList<ExternalInstitutionDto>>
{
    private readonly IExternalInstitutionService _service;

    public GetAllExternalInstitutionQueryHandler(IExternalInstitutionService service)
    {
        _service = service;
    }

    public async Task<IList<ExternalInstitutionDto>> Handle(
        GetAllExternalInstitutionQuery request,
        CancellationToken cancellationToken)
    {
        bool large = request.Type is null or ExternalInstitutionTypes.Institution;
        int? take = large
            ? Math.Clamp(request.Take ?? GetAllExternalInstitutionQuery.DefaultTake, 1, GetAllExternalInstitutionQuery.MaxTake)
            : null;

        var entities = await _service.GetAllAsync(request.Type, request.Search, take, cancellationToken);

        return entities
            .Select(x => new ExternalInstitutionDto
            {
                Id = x.Id,
                Name = x.Name,
                Type = x.Type,
                Address = x.Address,
                DetsisCode = x.DetsisCode,
                ParentId = x.ParentId
            })
            .ToList();
    }
}
