using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.ExternalInstitutionFeatures.Queries.GetExternalInstitutionById;

public sealed record GetExternalInstitutionByIdQuery(
    Guid Id
) : IRequest<ExternalInstitutionDto?>;