using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.LanguageFeatures.Queries.GetLanguageById;

public sealed record GetLanguageByIdQuery(
    Guid Id
) : IRequest<LanguageDto?>;
