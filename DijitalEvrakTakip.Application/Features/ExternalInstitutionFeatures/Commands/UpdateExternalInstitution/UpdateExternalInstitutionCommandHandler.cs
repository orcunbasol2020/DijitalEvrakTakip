using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.ExternalInstitutionFeatures.Commands.UpdateExternalInstitution;

public sealed class UpdateExternalInstitutionCommandHandler
    : IRequestHandler<UpdateExternalInstitutionCommand, MessageResponse>
{
    private readonly IExternalInstitutionService _externalInstitutionService;

    public UpdateExternalInstitutionCommandHandler(IExternalInstitutionService externalInstitutionService)
    {
        _externalInstitutionService = externalInstitutionService;
    }

    public async Task<MessageResponse> Handle(
        UpdateExternalInstitutionCommand request,
        CancellationToken cancellationToken)
    {
        var institution = await _externalInstitutionService.GetByIdAsync(request.Id, cancellationToken);

        if (institution is null)
            return new MessageResponse("Kurum kaydı bulunamadı");

        if (!string.IsNullOrWhiteSpace(request.Name))
            institution.Name = request.Name;

        if (request.Type.HasValue)
            institution.Type = request.Type.Value;

        if (!string.IsNullOrWhiteSpace(request.Address))
            institution.Address = request.Address;

        if (request.ParentId.HasValue)
            institution.ParentId = request.ParentId.Value;

        await _externalInstitutionService.UpdateAsync(institution, cancellationToken);

        return new MessageResponse("Kurum kaydı güncellendi");
    }
}
