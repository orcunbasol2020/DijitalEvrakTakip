using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.EnvelopeFeatures.Queries.GetAllEnvelope;

public sealed class GetAllEnvelopeHandler
    : IRequestHandler<GetAllEnvelopeQuery, IList<EnvelopeDocumentCountDto>>
{
    private readonly IEnvelopeService _envelopeService;
    private readonly IExternalInstitutionService _externalInstitutionService;

    public GetAllEnvelopeHandler(
        IEnvelopeService envelopeService,
        IExternalInstitutionService externalInstitutionService)
    {
        _envelopeService = envelopeService;
        _externalInstitutionService = externalInstitutionService;
    }

    public async Task<IList<EnvelopeDocumentCountDto>> Handle(
        GetAllEnvelopeQuery request,
        CancellationToken cancellationToken)
    {
        var envelopes = await _envelopeService.GetAllAsync(cancellationToken);

        var result = new List<EnvelopeDocumentCountDto>();

        foreach (var x in envelopes.Where(x => !x.IsDeleted))
        {
            string? institutionName = null;

            if (x.ExternalInstitutionId.HasValue)
            {
                var institution = await _externalInstitutionService
                    .GetByIdAsync(x.ExternalInstitutionId.Value, cancellationToken);

                institutionName = institution?.Name;
            }

            result.Add(new EnvelopeDocumentCountDto(
                x.Id,
                x.EnvelopeNo,
                x.IsClosed,
                x.Status,
                x.CreatedByUserId,
                x.ExternalInstitutionId,
                institutionName,
                x.DepartmentId,
                x.UnitName,
                x.Address,
                x.CreatedDate,
                x.EnvelopeDocuments != null ? x.EnvelopeDocuments.Count : 0
            ));
        }

        return result;
    }
}