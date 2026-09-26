using DijitalEvrakTakip.Domain.Constants;
using FluentValidation;

namespace DijitalEvrakTakip.Application.Features.AppSettingFeatures.Commands.UpdateAppSetting;

public sealed class UpdateAppSettingValidator : AbstractValidator<UpdateAppSettingCommand>
{
    public UpdateAppSettingValidator()
    {
        RuleFor(p => p.Key)
            .NotEmpty().WithMessage("Ayar anahtarı boş olamaz!")
            .Must(key => AppSettingKeys.All.Contains(key, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Geçersiz ayar anahtarı!");

        RuleFor(p => p.Value)
            .MaximumLength(2000).WithMessage("Ayar değeri 2000 karakterden fazla olamaz!")
            .When(p => p.Value is not null);

        RuleFor(p => p.Value)
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz!")
            .When(p => string.Equals(p.Key, AppSettingKeys.SupportEmail, StringComparison.OrdinalIgnoreCase)
                       && !string.IsNullOrWhiteSpace(p.Value));
    }
}
