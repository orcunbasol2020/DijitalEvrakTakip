using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.ExternalUserFeatures.Queries.GetAllExternalUser;

public sealed record GetAllExternalUserQuery() : IRequest<IList<ExternalUserDto>>;