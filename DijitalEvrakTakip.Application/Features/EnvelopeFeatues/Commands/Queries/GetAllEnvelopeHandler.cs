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
        var envelopes = await _envelopeService.GetAllAsync(request.CreatedByUserId, request.DepartmentId, cancellationToken);

        var result = new List<EnvelopeDocumentCountDto>();
        var activeEnvelopes = envelopes.Where(x => !x.IsDeleted).ToList();

        // Kurum adları tek sorguda; silinmiş kurumun adı da gelir
        var institutionNames = await _externalInstitutionService.GetNamesAsync(
            activeEnvelopes.Where(x => x.ExternalInstitutionId.HasValue).Select(x => x.ExternalInstitutionId!.Value),
            cancellationToken);

        foreach (var x in activeEnvelopes)
        {
            string? institutionName = x.ExternalInstitutionId.HasValue
                ? institutionNames.GetValueOrDefault(x.ExternalInstitutionId.Value)
                : null;

            result.Add(new EnvelopeDocumentCountDto(
                x.Id,
                x.EnvelopeNo,
                x.IsClosed,
                x.Status,
                x.CreatedByUserId,
                x.ExternalInstitutionId,
                institutionName,
                x.DepartmentId,
                x.TargetDepartmentId,
                x.UnitName,
                x.Address,
                x.CreatedDate,
                x.EnvelopeDocuments != null ? x.EnvelopeDocuments.Count : 0
            ));
        }

        return result;
    }
}