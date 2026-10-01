using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;
using Mapster;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.LanguageFeatures.Commands.CreateLanguage;

public sealed class CreateLanguageCommandHandler
    : IRequestHandler<CreateLanguageCommand, MessageResponse>
{
    private readonly ILanguageService _languageService;

    public CreateLanguageCommandHandler(ILanguageService languageService)
    {
        _languageService = languageService;
    }

    public async Task<MessageResponse> Handle(
        CreateLanguageCommand request,
        CancellationToken cancellationToken)
    {
        Language language = request.Adapt<Language>();

        await _languageService.CreateAsync(language, cancellationToken);

        return new("Dil başarıyla kaydedildi");
    }
}
