using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.ExternalInstitutionFeatures.Queries.GetExternalInstitutionById;

public sealed class GetExternalInstitutionByIdQueryHandler
    : IRequestHandler<GetExternalInstitutionByIdQuery, ExternalInstitutionDto?>
{
    private readonly IExternalInstitutionService _service;

    public GetExternalInstitutionByIdQueryHandler(IExternalInstitutionService service)
    {
        _service = service;
    }

    public async Task<ExternalInstitutionDto?> Handle(
        GetExternalInstitutionByIdQuery request,
        CancellationToken cancellationToken)
    {
        var entity = await _service.GetByIdAsync(request.Id, cancellationToken);

        if (entity == null || entity.IsDeleted)
            return null;

        return new ExternalInstitutionDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Type = entity.Type,
            Address = entity.Address
        };
    }
}