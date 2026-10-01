using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.LanguageFeatures.Commands.UpdateLanguage;

public sealed class UpdateLanguageCommandHandler
    : IRequestHandler<UpdateLanguageCommand, MessageResponse>
{
    private readonly ILanguageService _languageService;

    public UpdateLanguageCommandHandler(ILanguageService languageService)
    {
        _languageService = languageService;
    }

    public async Task<MessageResponse> Handle(
        UpdateLanguageCommand request,
        CancellationToken cancellationToken)
    {
        var language = await _languageService.GetByIdAsync(request.Id, cancellationToken);

        if (language is null)
            return new MessageResponse("Dil kaydı bulunamadı");

        if (!string.IsNullOrWhiteSpace(request.Name))
            language.Name = request.Name;

        if (request.OcrSupport.HasValue)
            language.OcrSupport = request.OcrSupport.Value;

        if (request.IsActive.HasValue)
            language.IsActive = request.IsActive.Value;

        await _languageService.UpdateAsync(language, cancellationToken);

        return new MessageResponse("Dil kaydı güncellendi");
    }
}
