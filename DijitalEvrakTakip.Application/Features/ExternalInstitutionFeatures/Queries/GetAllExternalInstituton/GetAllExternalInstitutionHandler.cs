using DijitalEvrakTakip.Application.Services;
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
        var entities = await _service.GetAllAsync(cancellationToken);

        return entities
            .Where(x => !x.IsDeleted)
            .Select(x => new ExternalInstitutionDto
            {
                Id = x.Id,
                Name = x.Name,
                Type= x.Type,
                Address = x.Address
            })
            .ToList();
    }
}
