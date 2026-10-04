using DijitalEvrakTakip.Application.Features.AppSettingFeatures.Commands.UpdateAppSetting;

namespace DijitalEvrakTakip.UnitTest
{
    public class UpdateAppSettingValidatorUnitTest
    {
        private readonly UpdateAppSettingValidator _validator = new();

        [Theory]
        [InlineData("AtlasNumberPoolMinStock", "100")]
        [InlineData("AtlasNumberPoolMinStock", " 100 ")]
        [InlineData("AtlasNumberPoolBatchSize", "1000")]
        [InlineData("AtlasNumberPoolIntervalSeconds", "30")]
        [InlineData("AtlasNumberPoolEnabled", "false")]
        [InlineData("AtlasNumberPoolEnforced", "True")]
        [InlineData("ScanImportIntervalSeconds", "10")]
        [InlineData("ZimmetReminderEscalateAfter", "0")]
        [InlineData("ZimmetReminderWorkStartHour", "0")]
        [InlineData("ZimmetReminderWorkEndHour", "24")]
        [InlineData("zimmetreminderworkendhour", "18")]
        [InlineData("ApplicationName", "Dijital Evrak Takip")]
        [InlineData("SupportPhone", null)]
        [InlineData("AnnouncementMessage", "")]
        [InlineData("SupportEmail", "")]
        public void Validate_Passes_WhenValueIsValid(string key, string? value)
        {
            var result = _validator.Validate(new UpdateAppSettingCommand(key, value, null));

            Assert.True(result.IsValid, string.Join(", ", result.Errors.Select(x => x.ErrorMessage)));
        }

        [Theory]
        [InlineData("AtlasNumberPoolMinStock", "abc")]
        [InlineData("AtlasNumberPoolMinStock", "0")]
        [InlineData("AtlasNumberPoolMinStock", "")]
        [InlineData("AtlasNumberPoolMinStock", null)]
        [InlineData("AtlasNumberPoolMinStock", "10.5")]
        [InlineData("AtlasNumberPoolBatchSize", "1001")]
        [InlineData("AtlasNumberPoolIntervalSeconds", "29")]
        [InlineData("AtlasNumberPoolEnabled", "evet")]
        [InlineData("AtlasNumberPoolEnforced", "1")]
        [InlineData("ScanImportIntervalSeconds", "9")]
        [InlineData("ScanImportFolderPath", " ")]
        [InlineData("ZimmetReminderIntervalHours", "0")]
        [InlineData("ZimmetReminderEscalateAfter", "-1")]
        [InlineData("ZimmetReminderWorkStartHour", "24")]
        [InlineData("ZimmetReminderWorkEndHour", "0")]
        [InlineData("ApplicationName", "")]
        [InlineData("SupportEmail", "gecersiz")]
        [InlineData("BilinmeyenAyar", "1")]
        public void Validate_Fails_WhenValueIsInvalid(string key, string? value)
        {
            var result = _validator.Validate(new UpdateAppSettingCommand(key, value, null));

            Assert.False(result.IsValid);
        }
    }
}
