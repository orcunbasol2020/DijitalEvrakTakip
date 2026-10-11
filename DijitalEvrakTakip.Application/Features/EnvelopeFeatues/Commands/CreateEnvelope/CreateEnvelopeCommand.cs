using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.EnvelopeFeatures.Commands.CreateEnvelope
{
    public sealed record CreateEnvelopeCommand(
        Guid CreatedByUserId,
        Guid? ExternalInstitutionId,
        int? DepartmentId,
        int? TargetDepartmentId,
        string? UnitName,
        string? Address
    ) : IRequest<EnvelopeReturnDto>;
}