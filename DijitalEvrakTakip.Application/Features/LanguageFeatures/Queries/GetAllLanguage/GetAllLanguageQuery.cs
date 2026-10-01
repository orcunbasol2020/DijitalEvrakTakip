using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.LanguageFeatures.Queries.GetAllLanguage;

public sealed record GetAllLanguageQuery()
    : IRequest<IList<LanguageDto>>;
