using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.EnvelopeFeatures.Queries.GetEnvelopeById;

public sealed class GetEnvelopeByIdHandler
    : IRequestHandler<GetEnvelopeByIdQuery, EnvelopeDto?>
{
    private readonly IEnvelopeService _envelopeService;
    private readonly IExternalInstitutionService _externalInstitutionService;

    public GetEnvelopeByIdHandler(
        IEnvelopeService envelopeService,
        IExternalInstitutionService externalInstitutionService)
    {
        _envelopeService = envelopeService;
        _externalInstitutionService = externalInstitutionService;
    }

    public async Task<EnvelopeDto?> Handle(
        GetEnvelopeByIdQuery request,
        CancellationToken cancellationToken)
    {
        var envelope = await _envelopeService.GetByIdAsync(request.Id, cancellationToken);

        if (envelope == null || envelope.IsDeleted)
            return null;

        string? institutionName = envelope.ExternalInstitutionId.HasValue
            ? (await _externalInstitutionService.GetNamesAsync(new[] { envelope.ExternalInstitutionId.Value }, cancellationToken))
                .GetValueOrDefault(envelope.ExternalInstitutionId.Value)
            : null;

        return new EnvelopeDto(
            envelope.Id,
            envelope.EnvelopeNo,
            envelope.IsClosed,
            envelope.Status,
            envelope.CreatedByUserId,
            envelope.ExternalInstitutionId,
            institutionName,
            envelope.DepartmentId,
            envelope.TargetDepartmentId,
            envelope.UnitName,
            envelope.Address,
            envelope.CreatedDate
        );
    }
}