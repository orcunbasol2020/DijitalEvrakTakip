using DijitalEvrakTakip.Application.Eyp;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Enums;

namespace DijitalEvrakTakip.UnitTest
{
    public class IncomingDocumentEypMapperUnitTest
    {
        private static readonly EypRecipient Recipient = new("12345678", "T.C. Dışişleri Bakanlığı");

        private static IncomingDocument CreateValidDocument()
        {
            var institution = new ExternalInstitution
            {
                Name = "Gönderen Kurum",
                Address = "Ankara",
                DetsisCode = "87654321"
            };

            return new IncomingDocument
            {
                QrCode = "2026/90000001",
                OrginalNo = "E-123",
                Subject = " Toplantı daveti ",
                DocumentDate = new DateTime(2026, 10, 1),
                SecurityDegree = (int)SecurityDegreeEnum.ServiceUseOnly,
                UrgencyDegree = (int)UrgencyDegreeEnum.VeryUrgent,
                ActionRequired = true,
                ExternalInstitutionId = institution.Id,
                ExternalInstitution = institution,
                UserId = "user"
            };
        }

        [Fact]
        public void Validate_ReturnsNoErrors_WhenDocumentIsComplete()
        {
            Assert.Empty(IncomingDocumentEypMapper.Validate(CreateValidDocument()));
        }

        [Fact]
        public void Validate_ReportsEveryMissingField()
        {
            var document = new IncomingDocument { QrCode = "", UserId = "user" };

            var errors = IncomingDocumentEypMapper.Validate(document);

            Assert.Equal(5, errors.Count);
        }

        [Fact]
        public void Validate_Fails_WhenInstitutionHasNoDetsisCode()
        {
            var document = CreateValidDocument();
            document.ExternalInstitution!.DetsisCode = " ";

            var errors = IncomingDocumentEypMapper.Validate(document);

            Assert.Single(errors);
            Assert.Contains("DETSİS", errors[0]);
        }

        [Fact]
        public void Validate_Fails_ForPersonalUseOnly_UntilMappingIsDecided()
        {
            var document = CreateValidDocument();
            document.SecurityDegree = (int)SecurityDegreeEnum.PersonalUseOnly;

            var errors = IncomingDocumentEypMapper.Validate(document);

            Assert.Single(errors);
            Assert.Contains("Kişiye Özel", errors[0]);
        }

        [Fact]
        public void Map_FillsSenderRecipientAndCodes()
        {
            var document = CreateValidDocument();

            var metadata = IncomingDocumentEypMapper.Map(document, Recipient);

            Assert.Equal(document.Id.ToString(), metadata.DocumentId);
            Assert.Equal("Toplantı daveti", metadata.Title);
            Assert.Equal("HZO", metadata.GizlilikRumuz);
            Assert.Equal("E-123", metadata.BelgeNo);
            Assert.Equal(document.DocumentDate, metadata.DocumentDate);
            Assert.Equal("87654321", metadata.OlusturanKkk);
            Assert.Equal("Gönderen Kurum", metadata.Sender);
            Assert.Equal("application/pdf", metadata.MimeType);

            var dagitim = Assert.Single(metadata.DagitimListesi);
            Assert.Equal("12345678", dagitim.KKK);
            Assert.Equal("GRG", dagitim.BilgiGeregiRumuz);
            Assert.Equal("CIV", dagitim.IvedilikRumuz);

            Assert.Empty(metadata.EkListesi);
        }

        [Fact]
        public void Map_UsesAtlasNumber_WhenOriginalNumberIsMissing()
        {
            var document = CreateValidDocument();
            document.OrginalNo = null;

            Assert.Equal("2026/90000001", IncomingDocumentEypMapper.Map(document, Recipient).BelgeNo);
        }

        [Fact]
        public void Map_AddsPhysicalAttachment_WhenDocumentHasAttachment()
        {
            var document = CreateValidDocument();
            document.HasAttachment = true;
            document.AttachmentDescription = "1 flash disk";

            var ek = Assert.Single(IncomingDocumentEypMapper.Map(document, Recipient).EkListesi);

            Assert.Equal("FZK", ek.Tur);
            Assert.Equal("1 flash disk", ek.Ad);
            Assert.Null(ek.DosyaIcerigi);
        }

        [Fact]
        public void Map_SetsBilgi_WhenActionIsNotRequired()
        {
            var document = CreateValidDocument();
            document.ActionRequired = false;

            Assert.Equal("BLG", IncomingDocumentEypMapper.Map(document, Recipient).DagitimListesi[0].BilgiGeregiRumuz);
        }

        [Fact]
        public void Map_Throws_WhenRecipientIsNotConfigured()
        {
            Assert.Throws<InvalidOperationException>(
                () => IncomingDocumentEypMapper.Map(CreateValidDocument(), new EypRecipient("", "")));
        }

        [Fact]
        public void Map_Throws_WhenInstitutionIsNotLoaded()
        {
            var document = CreateValidDocument();
            document.ExternalInstitution = null;

            Assert.Throws<InvalidOperationException>(() => IncomingDocumentEypMapper.Map(document, Recipient));
        }

        [Theory]
        [InlineData(null, "NRM")]
        [InlineData((int)UrgencyDegreeEnum.Normal, "NRM")]
        [InlineData((int)UrgencyDegreeEnum.Urgent, "IVD")]
        [InlineData((int)UrgencyDegreeEnum.Lightning, "YLDRM")]
        [InlineData((int)UrgencyDegreeEnum.Dated, "GNL")]
        [InlineData((int)UrgencyDegreeEnum.UrgentTimeLimited, "IVD")]
        public void ToIvedilikKodu_MapsUrgencyDegrees(int? urgencyDegree, string expected)
        {
            Assert.Equal(expected, IncomingDocumentEypMapper.ToIvedilikKodu(urgencyDegree));
        }

        [Theory]
        [InlineData((int)SecurityDegreeEnum.Unclassified, "YOK")]
        [InlineData((int)SecurityDegreeEnum.Special, "OZL")]
        [InlineData((int)SecurityDegreeEnum.Confidential, "GZL")]
        [InlineData((int)SecurityDegreeEnum.TopSecret, "CGZ")]
        [InlineData((int)SecurityDegreeEnum.PersonalUseOnly, null)]
        [InlineData(null, null)]
        public void ToGuvenlikKodu_MapsSecurityDegrees(int? securityDegree, string? expected)
        {
            Assert.Equal(expected, IncomingDocumentEypMapper.ToGuvenlikKodu(securityDegree));
        }
    }
}
