using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.EnvelopeFeatures.Commands.CreateEnvelope;

public sealed class CreateEnvelopeCommandHandler
    : IRequestHandler<CreateEnvelopeCommand, EnvelopeReturnDto>
{
    private readonly IEnvelopeService _envelopeService;

    public CreateEnvelopeCommandHandler(IEnvelopeService envelopeService)
    {
        _envelopeService = envelopeService;
    }

    public async Task<EnvelopeReturnDto> Handle(
      CreateEnvelopeCommand request,
      CancellationToken cancellationToken)
    {
        Envelope envelope = new()
        {
            EnvelopeNo = await _envelopeService.GenerateEnvelopeNoAsync(cancellationToken),
            CreatedByUserId = request.CreatedByUserId,
            ExternalInstitutionId = request.ExternalInstitutionId,
            DepartmentId = request.DepartmentId,
            UnitName = request.UnitName,
            Address = request.Address,
            IsClosed = false
        };

        await _envelopeService.CreateAsync(envelope, cancellationToken);

        // ExternalInstitution adı çek
        string? externalInstitutionName = null;
        if (envelope.ExternalInstitutionId.HasValue)
        {
            var externalInstitution = await _envelopeService.GetExternalInstitutionByIdAsync(
                envelope.ExternalInstitutionId.Value,
                cancellationToken
            );

            externalInstitutionName = externalInstitution?.Name;
        }

        return new EnvelopeReturnDto
        {
            Id = envelope.Id,
            EnvelopeNo = envelope.EnvelopeNo,
            UnitName = envelope.UnitName,
            Address = envelope.Address,
            ExternalInstitutionId = envelope.ExternalInstitutionId,
            ExternalInstitutionName = externalInstitutionName
        };
    }
}