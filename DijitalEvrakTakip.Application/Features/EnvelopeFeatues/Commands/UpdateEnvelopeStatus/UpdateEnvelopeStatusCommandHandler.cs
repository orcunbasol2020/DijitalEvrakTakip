using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.EnvelopeFeatures.Commands.UpdateEnvelopeStatus;

public sealed class UpdateEnvelopeStatusCommandHandler
    : IRequestHandler<UpdateEnvelopeStatusCommand, MessageResponse>
{
    private readonly IEnvelopeService _envelopeService;

    public UpdateEnvelopeStatusCommandHandler(IEnvelopeService envelopeService)
    {
        _envelopeService = envelopeService;
    }

    public async Task<MessageResponse> Handle(
        UpdateEnvelopeStatusCommand request,
        CancellationToken cancellationToken)
    {
        var envelope = await _envelopeService.GetByIdAsync(request.Id, cancellationToken);

        if (envelope is null)
            return new MessageResponse("Zarf bulunamadı");

        if (envelope.Status == request.Status)
            return new MessageResponse("Zarf zaten bu durumda");

        envelope.Status = request.Status;

        await _envelopeService.UpdateAsync(envelope, cancellationToken);

        return new MessageResponse("Zarf durumu güncellendi");
    }
}
