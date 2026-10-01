using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.LanguageFeatures.Queries.GetAllLanguage;

public sealed class GetAllLanguageHandler
    : IRequestHandler<GetAllLanguageQuery, IList<LanguageDto>>
{
    private readonly ILanguageService _languageService;

    public GetAllLanguageHandler(ILanguageService languageService)
    {
        _languageService = languageService;
    }

    public async Task<IList<LanguageDto>> Handle(
        GetAllLanguageQuery request,
        CancellationToken cancellationToken)
    {
        var languages = await _languageService.GetAllAsync(cancellationToken);

        return languages
            .Select(x => new LanguageDto(
                x.Id,
                x.Name,
                x.OcrSupport,
                x.IsActive
            ))
            .ToList();
    }
}
