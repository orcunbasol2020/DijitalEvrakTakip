using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.LanguageFeatures.Queries.GetLanguageById;

public sealed class GetLanguageByIdQueryHandler
    : IRequestHandler<GetLanguageByIdQuery, LanguageDto?>
{
    private readonly ILanguageService _languageService;

    public GetLanguageByIdQueryHandler(ILanguageService languageService)
    {
        _languageService = languageService;
    }

    public async Task<LanguageDto?> Handle(
        GetLanguageByIdQuery request,
        CancellationToken cancellationToken)
    {
        var language = await _languageService.GetByIdAsync(request.Id, cancellationToken);

        if (language is null)
            return null;

        return new LanguageDto(
            language.Id,
            language.Name,
            language.OcrSupport,
            language.IsActive
        );
    }
}
