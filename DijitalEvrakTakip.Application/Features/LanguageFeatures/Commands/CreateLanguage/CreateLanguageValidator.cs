using FluentValidation;

namespace DijitalEvrakTakip.Application.Features.LanguageFeatures.Commands.CreateLanguage;

public sealed class CreateLanguageValidator
    : AbstractValidator<CreateLanguageCommand>
{
    public CreateLanguageValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty().WithMessage("Dil adı boş olamaz!")
            .MaximumLength(100).WithMessage("Dil adı 100 karakterden fazla olamaz!");
    }
}
