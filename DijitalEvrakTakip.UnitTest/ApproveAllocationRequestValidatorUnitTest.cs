using DijitalEvrakTakip.Application.Features.DocumentAllocationRequestFeatures.Commands.ApproveAllocationRequest;

namespace DijitalEvrakTakip.UnitTest
{
    public class ApproveAllocationRequestValidatorUnitTest
    {
        private readonly ApproveAllocationRequestValidator _validator = new();

        [Theory]
        [InlineData(false, null)]
        [InlineData(false, "")]
        [InlineData(false, "Teşekkürler")]
        [InlineData(true, "Ek olarak belirtilen flash disk gelmedi")]
        public void Validate_Passes_WhenNoteMatchesDiscrepancy(bool hasDiscrepancy, string? note)
        {
            var result = _validator.Validate(new ApproveAllocationRequestCommand(Guid.NewGuid(), Guid.NewGuid().ToString(), hasDiscrepancy, note));

            Assert.True(result.IsValid, string.Join(", ", result.Errors.Select(x => x.ErrorMessage)));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Validate_Fails_WhenDiscrepancyHasNoNote(string? note)
        {
            var result = _validator.Validate(new ApproveAllocationRequestCommand(Guid.NewGuid(), Guid.NewGuid().ToString(), true, note));

            Assert.False(result.IsValid);
        }

        [Fact]
        public void Validate_Fails_WhenNoteIsTooLong()
        {
            var result = _validator.Validate(new ApproveAllocationRequestCommand(Guid.NewGuid(), Guid.NewGuid().ToString(), true, new string('a', 1001)));

            Assert.False(result.IsValid);
        }
    }
}
