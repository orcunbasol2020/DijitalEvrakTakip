using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using MediatR;

namespace DijitalEvrakTakip.Application.Features.LanguageFeatures.Commands.DeleteLanguage;

public sealed class DeleteLanguageCommandHandler
    : IRequestHandler<DeleteLanguageCommand, MessageResponse>
{
    private readonly ILanguageService _languageService;

    public DeleteLanguageCommandHandler(ILanguageService languageService)
    {
        _languageService = languageService;
    }

    public async Task<MessageResponse> Handle(
        DeleteLanguageCommand request,
        CancellationToken cancellationToken)
    {
        var language = await _languageService.GetByIdAsync(request.Id, cancellationToken);

        if (language is null)
            return new MessageResponse("Dil kaydı bulunamadı");

        await _languageService.DeleteAsync(language, cancellationToken);

        return new MessageResponse("Dil kaydı silindi");
    }
}
