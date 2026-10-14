using System.IO.Packaging;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using DijitalEvrakTakip.Application.Eyp;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Enums;
using DijitalEvrakTakip.Infrastructure.Eyp;
using Microsoft.Extensions.Logging.Abstractions;

namespace DijitalEvrakTakip.UnitTest
{
    /// <summary>
    /// EypService'in ürettiği paketi Cbddo.eYazisma'dan bağımsız olarak denetler: parça adları örnek
    /// paketlerdeki gibi mi, PaketOzeti ve NihaiOzet'teki her özet parçanın gerçek baytlarıyla tutuyor mu.
    /// </summary>
    public class EypServiceUnitTest
    {
        private static readonly XNamespace Tipler = "urn:dpt:eyazisma:schema:xsd:Tipler-2";

        private static readonly byte[] UstYazi = Encoding.ASCII.GetBytes("%PDF-1.4 ust yazi");
        private static readonly byte[] Ek1 = Encoding.ASCII.GetBytes("%PDF-1.4 ek 1");
        private static readonly byte[] Ek2 = Encoding.ASCII.GetBytes("%PDF-1.4 ek 2");

        private static IncomingDocument CreateDocument(string? detsisCode = "87654321") => new()
        {
            QrCode = "2026/90000001",
            OrginalNo = "E-123",
            Subject = "Toplantı daveti",
            DocumentDate = new DateTime(2026, 10, 1),
            SecurityDegree = (int)SecurityDegreeEnum.Confidential,
            UrgencyDegree = (int)UrgencyDegreeEnum.Urgent,
            HasAttachment = true,
            AttachmentDescription = "1 adet CD",
            ExternalInstitutionId = 1,
            ExternalInstitution = new ExternalInstitution { Id = 1, Name = "Gönderen Kurum", DetsisCode = detsisCode },
            UserId = "user"
        };

        private static async Task<byte[]> BuildAsync(IncomingDocument document)
        {
            var metadata = IncomingDocumentEypMapper.Map(
                document,
                new EypRecipient("12345678", "Bakanlık"),
                new[]
                {
                    new EypDistribution("80981219", "72610926", "Kapatılmış Birim", true,
                        new EypContact("Balgat", "0 312 000 00 00", null, null, null)),
                    new EypDistribution("91851323", "91851323", "Bilgi Birimi", false)
                },
                new[]
                {
                    new EypAttachment("Gündem", "Taslak", "Gündem Taslağı.pdf", Ek1),
                    new EypAttachment("Liste", null, "liste.pdf", Ek2)
                },
                new EypContact("Ankara", "0 312 111 11 11", null, null, "https://kurum.gov.tr"));

            var service = new EypService(NullLogger<EypService>.Instance);
            var created = await service.EypOlusturAsync(metadata, UstYazi);
            return await service.EypTamamlaAsync(created, metadata);
        }

        [Fact]
        public async Task Package_HasSamplePartLayout()
        {
            var document = CreateDocument();
            var parts = ReadParts(await BuildAsync(document));
            var id = document.Id.ToString().ToLowerInvariant();

            Assert.Equal(UstYazi, parts[$"/UstYazi/{id}.pdf"]);
            Assert.Equal(Ek1, parts[$"/Ekler/{id}_2.pdf"]);
            Assert.Equal(Ek2, parts[$"/Ekler/{id}_3.pdf"]);
            Assert.Contains("/Ustveri/Ustveri.xml", parts.Keys);
            Assert.Contains("/NihaiUstveri/NihaiUstveri.xml", parts.Keys);
            Assert.Contains("/PaketOzeti/PaketOzeti.xml", parts.Keys);
            Assert.Contains("/NihaiOzet/NihaiOzet.xml", parts.Keys);
            Assert.Contains("/Imzalar/ImzaCades.imz", parts.Keys);
        }

        [Theory]
        [InlineData("/PaketOzeti/PaketOzeti.xml", 4)]   // Ustveri, UstYazi, 2 ek
        [InlineData("/NihaiOzet/NihaiOzet.xml", 8)]     // + PaketOzeti, imza, NihaiUstveri, core
        public async Task EveryDigest_MatchesThePartBytes(string ozetPart, int expectedReferences)
        {
            var parts = ReadParts(await BuildAsync(CreateDocument()));
            var references = Xml(parts[ozetPart])
                .Descendants().Where(e => e.Name.LocalName == "Reference").ToList();

            Assert.Equal(expectedReferences, references.Count);
            foreach (var reference in references)
            {
                var content = parts[Uri.UnescapeDataString(reference.Attribute("URI")!.Value)];
                var items = reference.Elements().Where(e => e.Name.LocalName == "DigestItem").ToList();

                // Örnek paketlerdeki gibi SHA-384 + SHA-512
                Assert.Equal(2, items.Count);
                foreach (var item in items)
                {
                    var algorithm = item.Elements().Single(e => e.Name.LocalName == "DigestMethod").Attribute("Algorithm")!.Value;
                    var expected = item.Elements().Single(e => e.Name.LocalName == "DigestValue").Value.Trim();
                    var actual = algorithm.EndsWith("sha384") ? SHA384.HashData(content)
                        : algorithm.EndsWith("sha512") ? SHA512.HashData(content)
                        : throw new Xunit.Sdk.XunitException($"Beklenmeyen özet algoritması: {algorithm}");

                    Assert.Equal(expected, Convert.ToBase64String(actual));
                }
            }
        }

        [Fact]
        public async Task Ustveri_HasSenderDistributionAndAttachments()
        {
            var ustveri = Xml(ReadParts(await BuildAsync(CreateDocument()))["/Ustveri/Ustveri.xml"]);

            Assert.Equal("GZL", ustveri.Descendants(Tipler + "GuvenlikKodu").Single().Value);
            Assert.Equal("TUR", ustveri.Descendants(Tipler + "Dil").Single().Value);
            Assert.Equal("INT", ustveri.Descendants(Tipler + "OzId").Single().Attribute("schemeID")!.Value);

            var olusturan = ustveri.Descendants(Tipler + "Olusturan").Single();
            Assert.Equal("87654321", olusturan.Descendants(Tipler + "KKK").Single().Value);
            Assert.Equal("87654321", olusturan.Descendants(Tipler + "BirimKKK").Single().Value);
            Assert.Equal("0 312 111 11 11", olusturan.Descendants(Tipler + "Telefon").Single().Value);
            // Eksik iletişim bilgisi "-"
            Assert.Equal("-", olusturan.Descendants(Tipler + "Faks").Single().Value);

            var dagitimlar = ustveri.Descendants(Tipler + "Dagitim").ToList();
            Assert.Equal(2, dagitimlar.Count);
            Assert.Equal("80981219", dagitimlar[0].Descendants(Tipler + "KKK").Single().Value);
            Assert.Equal("72610926", dagitimlar[0].Descendants(Tipler + "BirimKKK").Single().Value);
            Assert.Equal("GRG", dagitimlar[0].Element(Tipler + "DagitimTuru")!.Value);
            Assert.Equal("IVD", dagitimlar[0].Element(Tipler + "Ivedilik")!.Value);
            Assert.Equal("BLG", dagitimlar[1].Element(Tipler + "DagitimTuru")!.Value);
            Assert.Equal("-", dagitimlar[1].Descendants(Tipler + "Adres").Single().Value);

            var ekler = ustveri.Descendants(Tipler + "Ek").ToList();
            Assert.Equal(new[] { "2", "3", "4" }, ekler.Select(e => e.Element(Tipler + "SiraNo")!.Value));
            Assert.Equal(new[] { "DED", "DED", "FZK" }, ekler.Select(e => e.Element(Tipler + "Tur")!.Value));
            Assert.Equal("Gündem Taslağı.pdf", ekler[0].Element(Tipler + "DosyaAdi")!.Value);
        }

        [Fact]
        public async Task SenderWithoutDetsisCode_GetsStandardCode()
        {
            var ustveri = Xml(ReadParts(await BuildAsync(CreateDocument(detsisCode: null)))["/Ustveri/Ustveri.xml"]);

            Assert.Equal("99999999", ustveri.Descendants(Tipler + "Olusturan").Descendants(Tipler + "KKK").Single().Value);
        }

        [Fact]
        public async Task Extract_ReadsBackDocumentInformation()
        {
            var document = CreateDocument();
            var service = new EypService(NullLogger<EypService>.Instance);

            var (metadata, content) = await service.ExtractEypPackageAsync(await BuildAsync(document));

            Assert.Equal(document.Id.ToString().ToUpperInvariant(), metadata.DocumentId);
            Assert.Equal("Toplantı daveti", metadata.Title);
            Assert.Equal("GZL", metadata.SecurityLevel);
            Assert.Equal("E-123", metadata.BelgeNo);
            Assert.Equal(document.DocumentDate, metadata.DocumentDate);
            Assert.Equal("Sistem Yöneticisi", metadata.ImzalayanAdi);
            Assert.Equal(2, metadata.DagitimListesi.Count);
            Assert.Equal(3, metadata.EkListesi.Count);
            Assert.Equal(UstYazi, content);
        }

        // Kütüphane XML'leri BOM'lu yazar; stream'den okunur
        private static XDocument Xml(byte[] content) => XDocument.Load(new MemoryStream(content));

        private static Dictionary<string, byte[]> ReadParts(byte[] eyp)
        {
            using var package = Package.Open(new MemoryStream(eyp), FileMode.Open, FileAccess.Read);
            var parts = new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);
            foreach (var part in package.GetParts())
            {
                using var stream = part.GetStream(FileMode.Open, FileAccess.Read);
                using var buffer = new MemoryStream();
                stream.CopyTo(buffer);
                parts[part.Uri.OriginalString] = buffer.ToArray();
            }
            return parts;
        }
    }
}
