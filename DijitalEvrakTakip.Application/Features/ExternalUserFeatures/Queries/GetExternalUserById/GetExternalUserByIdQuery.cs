using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.ExternalUserFeatures.Queries.GetExternalUserById;

public sealed record GetExternalUserByIdQuery(
    Guid Id
) : IRequest<ExternalUserDto?>;
