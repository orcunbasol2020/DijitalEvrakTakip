using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;
using Mapster;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.ExternalInstitutionFeatures.Commands.CreateExternalInstitution;

public sealed class CreateExternalInstitutionCommandHandler
    : IRequestHandler<CreateExternalInstitutionCommand, MessageResponse>
{
    private readonly IExternalInstitutionService _externalInstitutionService;

    public CreateExternalInstitutionCommandHandler(IExternalInstitutionService externalInstitutionService)
    {
        _externalInstitutionService = externalInstitutionService;
    }

    public async Task<MessageResponse> Handle(
        CreateExternalInstitutionCommand request,
        CancellationToken cancellationToken)
    {
        ExternalInstitution institution = request.Adapt<ExternalInstitution>();
        institution.Address = request.Address ?? string.Empty;

        await _externalInstitutionService.CreateAsync(institution, cancellationToken);

        return new("Kurum başarıyla kaydedildi");
    }
}
