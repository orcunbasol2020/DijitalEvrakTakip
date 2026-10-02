using DijitalEvrakTakip.Domain.Constants;
using FluentValidation;

namespace DijitalEvrakTakip.Application.Features.AppSettingFeatures.Commands.UpdateAppSetting;

public sealed class UpdateAppSettingValidator : AbstractValidator<UpdateAppSettingCommand>
{
    // Ayarı okuyan servisler geçersiz değerde sessizce varsayılana döndüğü için tip ve aralık burada kontrol edilir.
    // Ayarlar tek tek kaydedildiğinden ayarlar arası kurallar (mesai bitişi > başlangıcı) burada yok;
    // sıralı güncellemede meşru bir değişikliği engellerdi. O kontrol admin ekranında yapılıyor.
    private static readonly Dictionary<string, (Func<string?, bool> IsValid, string Message)> ValueRules =
        new(StringComparer.OrdinalIgnoreCase)
        {
            [AppSettingKeys.ApplicationName] = (NotEmpty, "Uygulama adı boş olamaz!"),

            [AppSettingKeys.ScanImportEnabled] = (IsBool, "Değer true veya false olmalıdır!"),
            [AppSettingKeys.ScanImportFolderPath] = (NotEmpty, "Tarama klasörü yolu boş olamaz!"),
            [AppSettingKeys.ScanImportIntervalSeconds] = (v => IsIntInRange(v, 10), "Kontrol aralığı en az 10 saniye olmalıdır!"),

            [AppSettingKeys.ZimmetApprovalRequired] = (IsBool, "Değer true veya false olmalıdır!"),
            [AppSettingKeys.ZimmetReminderEnabled] = (IsBool, "Değer true veya false olmalıdır!"),
            [AppSettingKeys.ZimmetReminderIntervalHours] = (v => IsIntInRange(v, 1), "Hatırlatma aralığı en az 1 saat olmalıdır!"),
            [AppSettingKeys.ZimmetReminderEscalateAfter] = (v => IsIntInRange(v, 0), "Değer 0 veya daha büyük bir tam sayı olmalıdır!"),
            [AppSettingKeys.ZimmetReminderWorkStartHour] = (v => IsIntInRange(v, 0, 23), "Başlangıç saati 0 ile 23 arasında olmalıdır!"),
            [AppSettingKeys.ZimmetReminderWorkEndHour] = (v => IsIntInRange(v, 1, 24), "Bitiş saati 1 ile 24 arasında olmalıdır!"),

            [AppSettingKeys.AtlasNumberPoolEnabled] = (IsBool, "Değer true veya false olmalıdır!"),
            [AppSettingKeys.AtlasNumberPoolEnforced] = (IsBool, "Değer true veya false olmalıdır!"),
            [AppSettingKeys.AtlasNumberPoolMinStock] = (v => IsIntInRange(v, 1), "Minimum stok en az 1 olmalıdır!"),
            [AppSettingKeys.AtlasNumberPoolBatchSize] = (v => IsIntInRange(v, 1, 1000), "Paket adedi 1 ile 1000 arasında olmalıdır!"),
            [AppSettingKeys.AtlasNumberPoolIntervalSeconds] = (v => IsIntInRange(v, 30), "Kontrol aralığı en az 30 saniye olmalıdır!")
        };

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

        RuleFor(p => p.Value)
            .Must((command, value) => ValueRules[command.Key].IsValid(value?.Trim()))
            .WithMessage(command => ValueRules[command.Key].Message)
            .When(p => p.Key is not null && ValueRules.ContainsKey(p.Key));
    }

    private static bool NotEmpty(string? value) => !string.IsNullOrWhiteSpace(value);

    private static bool IsBool(string? value) => bool.TryParse(value, out _);

    private static bool IsIntInRange(string? value, int min, int max = int.MaxValue) =>
        int.TryParse(value, out var number) && number >= min && number <= max;
}
