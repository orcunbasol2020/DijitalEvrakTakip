using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.EnvelopeFeatures.Commands.UpdateEnvelope;

public sealed class UpdateEnvelopeCommandHandler
    : IRequestHandler<UpdateEnvelopeCommand, MessageResponse>
{
    private readonly IEnvelopeService _envelopeService;

    public UpdateEnvelopeCommandHandler(IEnvelopeService envelopeService)
    {
        _envelopeService = envelopeService;
    }

    public async Task<MessageResponse> Handle(
        UpdateEnvelopeCommand request,
        CancellationToken cancellationToken)
    {
        var envelope = await _envelopeService.GetForUpdateAsync(request.Id, cancellationToken);

        if (envelope is null)
            throw new Exception("Zarf bulunamadı");

        if (request.UnitName != null)
            envelope.UnitName = string.IsNullOrWhiteSpace(request.UnitName) ? null : request.UnitName.Trim();

        if (request.Address != null)
            envelope.Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim();

        // Hedef tek olmalı: dış kurum seçildiyse kurum içi hedef temizlenir.
        if (request.ExternalInstitutionId.HasValue && request.ExternalInstitutionId.Value != Guid.Empty)
        {
            envelope.ExternalInstitutionId = request.ExternalInstitutionId.Value;
            envelope.TargetDepartmentId = null;
        }

        await _envelopeService.UpdateAsync(envelope, cancellationToken);

        return new MessageResponse("Zarf bilgileri güncellendi");
    }
}
