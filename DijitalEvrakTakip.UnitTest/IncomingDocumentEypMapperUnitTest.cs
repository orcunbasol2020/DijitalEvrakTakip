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

        // Yabancı misyonların ve dilekçelerin DETSİS kodu yok; standart kod yazılır
        [Fact]
        public void InstitutionWithoutDetsisCode_IsAccepted_AndGetsStandardCode()
        {
            var document = CreateValidDocument();
            document.ExternalInstitution!.DetsisCode = " ";

            Assert.Empty(IncomingDocumentEypMapper.Validate(document));
            Assert.Equal("99999999", IncomingDocumentEypMapper.Map(document, Recipient).OlusturanKkk);
        }

        [Fact]
        public void Map_FillsSenderContact_FromComAgency()
        {
            var contact = new EypContact(" Atatürk Bulvarı ", "0 312 000 00 00", null, "info@kurum.gov.tr", null);

            var metadata = IncomingDocumentEypMapper.Map(CreateValidDocument(), Recipient, senderContact: contact);

            Assert.Equal("Atatürk Bulvarı", metadata.OlusturanAdres);
            Assert.Equal("0 312 000 00 00", metadata.OlusturanTelefon);
            Assert.Null(metadata.OlusturanFaks);
            Assert.Equal("info@kurum.gov.tr", metadata.OlusturanEPosta);
        }

        // ComAgency kaydı olmayan gönderende (dilekçe) adres view'dan gelir
        [Fact]
        public void Map_FallsBackToInstitutionAddress_WithoutContact()
        {
            Assert.Equal("Ankara", IncomingDocumentEypMapper.Map(CreateValidDocument(), Recipient).OlusturanAdres);
        }

        [Fact]
        public void Map_UsesNereyeUnitsAsDistribution_WithTheirOwnBilgiGeregi()
        {
            var document = CreateValidDocument();
            var distributions = new[]
            {
                new EypDistribution("80981219", "72610926", "BDHD Bilişim Destek Hizmetleri Daire Başkanlığı", true,
                    new EypContact("Balgat", null, null, null, null)),
                new EypDistribution("91851323", "91851323", "BTGM Bilgi Teknolojileri Genel Müdürlüğü", false)
            };

            var dagitimlar = IncomingDocumentEypMapper.Map(document, Recipient, distributions).DagitimListesi;

            Assert.Equal(2, dagitimlar.Count);

            Assert.Equal("80981219", dagitimlar[0].KKK);
            Assert.Equal("72610926", dagitimlar[0].BirimKKK);
            Assert.Equal("BDHD Bilişim Destek Hizmetleri Daire Başkanlığı", dagitimlar[0].KurumAdi);
            Assert.Equal("GRG", dagitimlar[0].BilgiGeregiRumuz);
            Assert.Equal("CIV", dagitimlar[0].IvedilikRumuz);
            Assert.Equal("Balgat", dagitimlar[0].Adres);

            Assert.Equal("91851323", dagitimlar[1].KKK);
            Assert.Equal("BLG", dagitimlar[1].BilgiGeregiRumuz);
            Assert.Equal("CIV", dagitimlar[1].IvedilikRumuz);
        }

        // "Nereye" doluyken ayardaki alıcı kullanılmadığı için ayar eksikliği paketi durdurmaz
        [Fact]
        public void Map_DoesNotNeedRecipient_WhenNereyeIsFilled()
        {
            var metadata = IncomingDocumentEypMapper.Map(
                CreateValidDocument(),
                new EypRecipient("", ""),
                new[] { new EypDistribution("91851323", "91851323", "BTGM", null) });

            Assert.Equal("91851323", Assert.Single(metadata.DagitimListesi).KKK);
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
            Assert.Null(dagitim.BirimKKK);
            Assert.Equal("T.C. Dışişleri Bakanlığı", dagitim.KurumAdi);
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
            Assert.Equal(2, ek.SiraNo);
            Assert.Equal("1 flash disk", ek.Ad);
            Assert.Null(ek.DosyaIcerigi);
        }

        [Fact]
        public void Map_AddsElectronicAttachmentsInOrder_ThenPhysicalAttachment()
        {
            var document = CreateValidDocument();
            document.HasAttachment = true;
            document.AttachmentDescription = "1 dergi";
            var first = new byte[] { 1 };
            var second = new byte[] { 2 };

            var ekler = IncomingDocumentEypMapper.Map(document, Recipient, attachments: new[]
            {
                new EypAttachment("Toplantı gündemi", "Taslak", "Gündem Taslağı.pdf", first),
                new EypAttachment("Katılımcı listesi", null, "katilimci.pdf", second)
            }).EkListesi;

            Assert.Equal(3, ekler.Count);

            // Sıra 1 üst yazıdır; ekler 2'den başlar, üstveride ekin özgün dosya adı görünür
            Assert.Equal("DED", ekler[0].Tur);
            Assert.Equal(2, ekler[0].SiraNo);
            Assert.Equal("Toplantı gündemi", ekler[0].Ad);
            Assert.Equal("Taslak", ekler[0].Aciklama);
            Assert.Equal("Gündem Taslağı.pdf", ekler[0].DosyaAdi);
            Assert.Equal("application/pdf", ekler[0].MimeTuru);
            Assert.True(ekler[0].ImzaliMi);
            Assert.Same(first, ekler[0].DosyaIcerigi);

            Assert.Equal("DED", ekler[1].Tur);
            Assert.Equal(3, ekler[1].SiraNo);
            Assert.Equal("katilimci.pdf", ekler[1].DosyaAdi);
            Assert.Same(second, ekler[1].DosyaIcerigi);

            Assert.Equal("FZK", ekler[2].Tur);
            Assert.Equal(4, ekler[2].SiraNo);
            Assert.Equal("1 dergi", ekler[2].Ad);
        }

        // "Ek var" işareti elektronik ek yüklenince de konur; fiziki ek açıklaması yoksa FZK eklenmez
        [Fact]
        public void Map_SkipsPhysicalAttachment_WhenOnlyElectronicAttachmentsExist()
        {
            var document = CreateValidDocument();
            document.HasAttachment = true;

            var ek = Assert.Single(IncomingDocumentEypMapper.Map(
                document, Recipient, attachments: new[] { new EypAttachment("Ek", null, "ek.pdf", new byte[] { 1 }) }).EkListesi);

            Assert.Equal("DED", ek.Tur);
        }

        [Fact]
        public void Map_SetsBilgi_WhenActionIsNotRequired()
        {
            var document = CreateValidDocument();
            document.ActionRequired = false;

            Assert.Equal("BLG", IncomingDocumentEypMapper.Map(document, Recipient).DagitimListesi[0].BilgiGeregiRumuz);
        }

        [Fact]
        public void Map_Throws_WhenNereyeIsEmptyAndRecipientIsNotConfigured()
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
