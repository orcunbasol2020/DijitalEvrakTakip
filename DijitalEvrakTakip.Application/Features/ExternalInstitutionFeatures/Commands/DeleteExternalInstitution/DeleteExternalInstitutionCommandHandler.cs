using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.ExternalInstitutionFeatures.Commands.DeleteExternalInstitution;

public sealed class DeleteExternalInstitutionCommandHandler
    : IRequestHandler<DeleteExternalInstitutionCommand, MessageResponse>
{
    private readonly IExternalInstitutionService _externalInstitutionService;

    public DeleteExternalInstitutionCommandHandler(IExternalInstitutionService externalInstitutionService)
    {
        _externalInstitutionService = externalInstitutionService;
    }

    public async Task<MessageResponse> Handle(
        DeleteExternalInstitutionCommand request,
        CancellationToken cancellationToken)
    {
        var institution = await _externalInstitutionService.GetByIdAsync(request.Id, cancellationToken);

        if (institution is null)
            return new MessageResponse("Kurum kaydı bulunamadı");

        await _externalInstitutionService.DeleteAsync(institution, cancellationToken);

        return new MessageResponse("Kurum kaydı silindi");
    }
}
