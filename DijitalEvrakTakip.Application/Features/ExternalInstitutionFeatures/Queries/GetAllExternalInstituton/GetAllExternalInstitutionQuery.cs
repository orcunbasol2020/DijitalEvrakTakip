using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.ExternalInstitutionFeatures.Queries.GetAllExternalInstitution;

public sealed record GetAllExternalInstitutionQuery()
    : IRequest<IList<ExternalInstitutionDto>>;
