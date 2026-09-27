using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.EnvelopeFeatures.Commands.CreateEnvelope
{
    public sealed record CreateEnvelopeCommand(
        Guid CreatedByUserId,
        Guid? ExternalInstitutionId,
        Guid? DepartmentId,
        Guid? TargetDepartmentId,
        string? UnitName,
        string? Address
    ) : IRequest<EnvelopeReturnDto>;
}