using FluentValidation;

namespace DijitalEvrakTakip.Application.Features.LanguageFeatures.Commands.UpdateLanguage;

public sealed class UpdateLanguageValidator
    : AbstractValidator<UpdateLanguageCommand>
{
    public UpdateLanguageValidator()
    {
        RuleFor(p => p.Id)
            .NotEmpty().WithMessage("Dil Id boş olamaz!");

        RuleFor(p => p.Name)
            .MaximumLength(100).WithMessage("Dil adı 100 karakterden fazla olamaz!")
            .When(p => p.Name is not null);
    }
}
