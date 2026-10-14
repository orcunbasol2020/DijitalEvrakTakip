using System.IO.Packaging;
using System.Reflection;
using System.Security.Cryptography;
using Cbddo.eYazisma.Tipler;
using Cbddo.eYazisma.Xsd;
using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using Microsoft.Extensions.Logging;

namespace DijitalEvrakTakip.Infrastructure.Eyp;

/// <summary>
/// Cbddo.eYazisma kütüphanesiyle e-Yazışma 2.0 standardında EYP paketi üretir ve okur.
/// Paket yapısı örnek paketlerle aynıdır: UstYazi/&lt;BelgeId&gt;.pdf, Ekler/&lt;BelgeId&gt;_&lt;SiraNo&gt;.pdf,
/// özetler SHA-384 + SHA-512. İmza sahtedir: evrak kayıt birimi dışarıdan gelen evrakı paketler,
/// Atlas imzayı doğrulamaz. Eksik iletişim bilgisi "-" yazılır.
/// </summary>
public sealed class EypService : IEypService
{
    private const string DogrulamaAdresi = "https://e-belge.turkiye.gov.tr";
    private const string BosAlan = "-";
    private const string UstYaziIliskisi = "http://eyazisma.dpt/iliskiler/ustyazi";

    private readonly ILogger<EypService> _logger;

    public EypService(ILogger<EypService> logger)
    {
        _logger = logger;
    }

    public Task<Tuple<string, byte[]>> EypOlusturAsync(EypMetadataDto metadata, byte[]? fileContent)
    {
        if (fileContent is null || fileContent.Length == 0)
            throw new ArgumentException("Üst yazı dosyası verilmelidir.", nameof(fileContent));
        if (string.IsNullOrWhiteSpace(metadata.Title))
            throw new InvalidOperationException("EYP oluşturulamadı: konu yok.");
        if (string.IsNullOrWhiteSpace(metadata.OlusturanKkk) || string.IsNullOrWhiteSpace(metadata.Sender))
            throw new InvalidOperationException("EYP oluşturulamadı: oluşturan kurumun KKK kodu veya adı yok.");
        if (metadata.DagitimListesi.Count == 0)
            throw new InvalidOperationException($"Dağıtım listesi boş, EYP oluşturulamadı: {metadata.DocumentId}");

        var belgeId = Guid.TryParse(metadata.DocumentId, out var parsed) ? parsed : Guid.NewGuid();
        // Örnek paketlerde BelgeId büyük harf, paket içi dosya adları küçük harf
        var dosyaKoku = belgeId.ToString().ToLowerInvariant();
        var ustYaziAdi = dosyaKoku + Uzanti(metadata.FileName, metadata.MimeType);

        using var paketStream = new MemoryStream();
        using (var paket = Paket.Ac(paketStream, PaketModu.Olustur))
        {
            paket.Ustveri.BelgeIdBelirle(belgeId);
            paket.Ustveri.KonuBelirle(new TextType { Value = metadata.Title.Trim() });
            paket.Ustveri.GuvenlikKoduBelirle(GuvenlikKodu(metadata.GizlilikRumuz));
            // Örneklerdeki gibi saat dilimi eki olmadan
            paket.Ustveri.GuvenlikGecerlilikTarihiBelirle(DateTime.SpecifyKind(DateTime.Now.AddYears(10), DateTimeKind.Unspecified));
            paket.Ustveri.OzIdBelirle(belgeId.ToString(), "INT");
            paket.Ustveri.DilBelirle("TUR");
            paket.Ustveri.DogrulamaBilgisiEkle(new CT_DogrulamaBilgisi { DogrulamaAdresi = DogrulamaAdresi });

            var olusturanKkk = metadata.OlusturanKkk.Trim();
            paket.Ustveri.OlusturanBelirle(new CT_KurumKurulus
            {
                KKK = olusturanKkk,
                Adi = new NameType { Value = metadata.Sender.Trim() },
                IletisimBilgisi = Iletisim(metadata.OlusturanAdres, metadata.OlusturanTelefon, metadata.OlusturanFaks,
                    metadata.OlusturanEPosta, metadata.OlusturanWebAdresi),
                BirimKKK = olusturanKkk
            });

            foreach (var d in metadata.DagitimListesi)
            {
                paket.Ustveri.DagitimEkle(new CT_Dagitim
                {
                    DagitimTuru = DagitimTuru(d.BilgiGeregiRumuz),
                    Ivedilik = Ivedilik(d.IvedilikRumuz),
                    Item = new CT_KurumKurulus
                    {
                        KKK = d.KKK.Trim(),
                        Adi = new NameType { Value = d.KurumAdi.Trim() },
                        IletisimBilgisi = Iletisim(d.Adres, d.Telefon, d.Faks, d.EPosta, d.WebAdresi),
                        BirimKKK = string.IsNullOrWhiteSpace(d.BirimKKK) ? d.KKK.Trim() : d.BirimKKK.Trim()
                    }
                });
            }

            paket.Ustveri.DosyaAdiBelirle(ustYaziAdi);

            foreach (var ilgiDto in metadata.IlgiListesi)
            {
                var ilgi = new CT_Ilgi
                {
                    Id = new CT_Id { Value = string.IsNullOrEmpty(ilgiDto.Id) ? Guid.NewGuid().ToString().ToUpperInvariant() : ilgiDto.Id },
                    BelgeNo = ilgiDto.BelgeNo,
                    Etiket = ilgiDto.IlgiAcikMesajId
                };
                if (ilgiDto.Tarih.HasValue)
                {
                    ilgi.Tarih = ilgiDto.Tarih.Value;
                    ilgi.TarihSpecified = true;
                }
                if (!string.IsNullOrEmpty(ilgiDto.Ad))
                    ilgi.Ad = new TextType { Value = ilgiDto.Ad };
                if (!string.IsNullOrEmpty(ilgiDto.Aciklama))
                    ilgi.Aciklama = new TextType { Value = ilgiDto.Aciklama };
                paket.Ustveri.IlgiEkle(ilgi);
            }

            foreach (var ekDto in metadata.EkListesi.OrderBy(x => x.SiraNo))
            {
                var elektronik = ekDto.DosyaIcerigi is { Length: > 0 };
                var ek = new CT_Ek
                {
                    Id = new CT_Id { Value = string.IsNullOrEmpty(ekDto.Id) ? Guid.NewGuid().ToString().ToUpperInvariant() : ekDto.Id },
                    BelgeNo = ekDto.BelgeNo,
                    Tur = EkTuru(ekDto.Tur),
                    DosyaAdi = ekDto.DosyaAdi,
                    MimeTuru = ekDto.MimeTuru,
                    SiraNo = ekDto.SiraNo,
                    // Kütüphane imzalı ekleri Ekler/, imzasızları ImzasizEkler/ klasörüne koyar; örnekler Ekler/ kullanıyor
                    ImzaliMi = elektronik,
                    ImzaliMiSpecified = elektronik
                };
                if (!string.IsNullOrEmpty(ekDto.Ad))
                    ek.Ad = new TextType { Value = ekDto.Ad };
                if (!string.IsNullOrEmpty(ekDto.Aciklama))
                    ek.Aciklama = new TextType { Value = ekDto.Aciklama };

                // Ek üstveriye her zaman yazılır; elektronik ekin dosyası ayrıca pakete eklenir
                paket.Ustveri.EkEkle(ek);
                if (elektronik)
                {
                    using var ekStream = new MemoryStream(ekDto.DosyaIcerigi!);
                    paket.EkEkle(ek, ekStream, $"{dosyaKoku}_{ekDto.SiraNo}{Uzanti(ekDto.DosyaAdi, ekDto.MimeTuru)}", OzetModu.SHA384);
                }
            }

            using (var ustYazi = new MemoryStream(fileContent))
                paket.UstYaziEkle(ustYazi, ustYaziAdi, metadata.MimeType ?? "application/pdf", OzetModu.SHA384);

            paket.UstveriOlustur();
            paket.EkleriKontrolEt();
            paket.IlgileriKontrolEt();

            CloseInnerStreams(paket);
            paket.Kapat();
        }

        _logger.LogDebug("EYP paketi oluşturuldu (aşama 1): {BelgeId}", belgeId);
        return Task.FromResult(Tuple.Create(belgeId.ToString().ToUpperInvariant(), paketStream.ToArray()));
    }

    public async Task<byte[]> EypTamamlaAsync(Tuple<string, byte[]> eyp, EypMetadataDto eypMetadataDto)
    {
        if (string.IsNullOrWhiteSpace(eypMetadataDto.BelgeNo) || eypMetadataDto.DocumentDate is null)
            throw new InvalidOperationException("EYP tamamlanamadı: belge numarası ve tarih zorunludur.");

        // Kütüphane özetleri kendisine verilen stream'lerden hesaplar. .NET'te güncelleme modunda açılan
        // paketin bir parçası aynı açılışta ikinci kez açılamadığı için özetlenecek parçalar, paket
        // açılmadan önce salt okunur kopyadan okunur ve iş iki açılışa bölünür.
        var (ustveri, ustYazi) = (Parca(eyp.Item2, UstveriUri), UstYazi(eyp.Item2));

        var tempPath = Path.Combine(Path.GetTempPath(), $"eyp_{Guid.NewGuid():N}.eyp");
        await File.WriteAllBytesAsync(tempPath, eyp.Item2);
        try
        {
            // 1. açılış: nihai üstveri, paket özeti, core, imza
            using (var paket = Paket.Ac(tempPath, PaketModu.Guncelle))
            {
                paket.NihaiUstveri.BelgeNoBelirle(eypMetadataDto.BelgeNo);
                paket.NihaiUstveri.TarihBelirle(eypMetadataDto.DocumentDate.Value);
                paket.NihaiUstveri.ImzaEkle(new CT_Imza
                {
                    Imzalayan = new CT_GercekSahis
                    {
                        Kisi = new CT_Kisi
                        {
                            IlkAdi = new NameType { Value = "Sistem" },
                            Soyadi = new NameType { Value = "Yöneticisi" }
                        }
                    }
                });
                paket.NihaiUstveriOlustur();

                byte[] paketOzeti;
                using (var ustYaziStream = new MemoryStream(ustYazi))
                using (var ustveriStream = new MemoryStream(ustveri))
                    paketOzeti = BastanOku(paket.PaketOzetiOlustur(ustYaziStream, ustveriStream));

                paket.CoreOlustur();
                paket.ImzaEkle(SahteImza(paketOzeti));

                CloseInnerStreams(paket);
                paket.Kapat();
            }

            // 2. açılış: nihai özet, ilk açılışın kapatılmış çıktısındaki gerçek parçalardan
            var ara = await File.ReadAllBytesAsync(tempPath);
            using (var paket = Paket.Ac(tempPath, PaketModu.Guncelle))
            {
                using var ustYaziStream = new MemoryStream(ustYazi);
                using var ustveriStream = new MemoryStream(ustveri);
                using var imzaStream = new MemoryStream(Parca(ara, ImzaUri));
                using var nihaiUstveriStream = new MemoryStream(Parca(ara, NihaiUstveriUri));
                using var paketOzetiStream = new MemoryStream(Parca(ara, PaketOzetiUri));

                paket.NihaiOzetOlustur(
                    ustYaziStream: ustYaziStream,
                    ustVeriStream: ustveriStream,
                    imzaStream: imzaStream,
                    nihaiUstVeriStream: nihaiUstveriStream,
                    paketOzetiStream: paketOzetiStream);

                CloseInnerStreams(paket);
                paket.Kapat();
            }

            _logger.LogDebug("EYP paketi tamamlandı (aşama 2): {BelgeId}", eyp.Item1);
            return await File.ReadAllBytesAsync(tempPath);
        }
        finally
        {
            File.Delete(tempPath);
        }
    }

    public async Task<(EypMetadataDto Metadata, byte[]? FileContent)> ExtractEypPackageAsync(byte[] eypPackage)
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"eyp_{Guid.NewGuid():N}.eyp");
        await File.WriteAllBytesAsync(tempPath, eypPackage);
        try
        {
            using var paket = Paket.Ac(tempPath, PaketModu.Ac);

            var metadata = new EypMetadataDto
            {
                DocumentId = paket.Ustveri.BelgeIdAl() ?? string.Empty,
                Title = paket.Ustveri.KonuAl()?.Value,
                MimeType = paket.Ustveri.MimeTuruAl(),
                SecurityLevel = paket.Ustveri.GuvenlikKoduAl().ToString(),
                FileName = paket.Ustveri.DosyaAdiAl()
            };
            metadata.Konu = metadata.Title;

            if (paket.Ustveri.OlusturanAl()?.Item is CT_KurumKurulus olusturan)
            {
                metadata.Sender = olusturan.Adi?.Value;
                metadata.OlusturanKkk = olusturan.KKK;
                metadata.OlusturanAdres = olusturan.IletisimBilgisi?.Adres?.Value;
                metadata.OlusturanTelefon = olusturan.IletisimBilgisi?.Telefon;
                metadata.OlusturanFaks = olusturan.IletisimBilgisi?.Faks;
                metadata.OlusturanEPosta = olusturan.IletisimBilgisi?.EPosta;
                metadata.OlusturanWebAdresi = olusturan.IletisimBilgisi?.WebAdresi;
            }

            foreach (var dagitim in paket.Ustveri.DagitimlariAl() ?? Array.Empty<CT_Dagitim>())
            {
                if (dagitim.Item is not CT_KurumKurulus kurum)
                    continue;

                metadata.Receiver ??= kurum.Adi?.Value;
                metadata.DagitimListesi.Add(new DagitimDto
                {
                    KKK = kurum.KKK ?? string.Empty,
                    BirimKKK = kurum.BirimKKK,
                    KurumAdi = kurum.Adi?.Value ?? string.Empty,
                    DagitimTuru = dagitim.DagitimTuru.ToString(),
                    Ivedilik = dagitim.Ivedilik.ToString()
                });
            }

            foreach (var ilgi in paket.Ustveri.IlgileriAl() ?? Array.Empty<CT_Ilgi>())
            {
                metadata.IlgiListesi.Add(new IlgiDto
                {
                    Id = ilgi.Id?.Value,
                    BelgeNo = ilgi.BelgeNo,
                    Tarih = ilgi.TarihSpecified ? ilgi.Tarih : null,
                    Etiket = ilgi.Etiket,
                    Ad = ilgi.Ad?.Value,
                    Aciklama = ilgi.Aciklama?.Value
                });
            }

            foreach (var ek in paket.Ustveri.EkleriAl() ?? Array.Empty<CT_Ek>())
            {
                metadata.EkListesi.Add(new EkDto
                {
                    Id = ek.Id?.Value,
                    BelgeNo = ek.BelgeNo,
                    Tur = ek.Tur.ToString(),
                    DosyaAdi = ek.DosyaAdi,
                    MimeTuru = ek.MimeTuru,
                    Ad = ek.Ad?.Value,
                    SiraNo = ek.SiraNo,
                    Aciklama = ek.Aciklama?.Value,
                    ImzaliMi = ek.ImzaliMiSpecified && ek.ImzaliMi
                });
            }

            if (paket.NihaiUstveriVarMi())
            {
                metadata.BelgeNo = paket.NihaiUstveri.BelgeNoAl();
                metadata.DocumentDate = paket.NihaiUstveri.TarihAl();

                var imzalayan = paket.NihaiUstveri.ImzalariAl()?.FirstOrDefault()?.Imzalayan;
                if (imzalayan?.Kisi is not null)
                {
                    metadata.ImzalayanAdi = $"{imzalayan.Kisi.IlkAdi?.Value} {imzalayan.Kisi.Soyadi?.Value}".Trim();
                    metadata.ImzalayanUnvan = imzalayan.Gorev?.Value;
                }
            }

            byte[]? fileContent = null;
            using (var ustYazi = paket.UstYaziAl())
            {
                if (ustYazi is not null)
                {
                    using var buffer = new MemoryStream();
                    await ustYazi.CopyToAsync(buffer);
                    fileContent = buffer.ToArray();
                    metadata.FileSize = fileContent.Length;
                }
            }

            return (metadata, fileContent);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"EYP paketi ayrıştırılamadı: {ex.Message}", ex);
        }
        finally
        {
            File.Delete(tempPath);
        }
    }

    public async Task<bool> ValidateEypPackageAsync(byte[] eypPackage)
    {
        try
        {
            var (metadata, _) = await ExtractEypPackageAsync(eypPackage);
            return !string.IsNullOrEmpty(metadata.DocumentId);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "EYP paketi geçersiz");
            return false;
        }
    }

    private static readonly Uri UstveriUri = new("/Ustveri/Ustveri.xml", UriKind.Relative);
    private static readonly Uri NihaiUstveriUri = new("/NihaiUstveri/NihaiUstveri.xml", UriKind.Relative);
    private static readonly Uri PaketOzetiUri = new("/PaketOzeti/PaketOzeti.xml", UriKind.Relative);
    private static readonly Uri ImzaUri = new("/Imzalar/ImzaCades.imz", UriKind.Relative);

    /// <summary>Paketin bir parçasını salt okunur açarak okur.</summary>
    private static byte[] Parca(byte[] eyp, Uri uri)
    {
        using var package = Package.Open(new MemoryStream(eyp), FileMode.Open, FileAccess.Read);
        return ParcaOku(package, uri);
    }

    private static byte[] UstYazi(byte[] eyp)
    {
        using var package = Package.Open(new MemoryStream(eyp), FileMode.Open, FileAccess.Read);
        var uri = package.GetRelationshipsByType(UstYaziIliskisi).Single().TargetUri;
        return ParcaOku(package, uri);
    }

    private static byte[] ParcaOku(Package package, Uri uri)
    {
        using var stream = package.GetPart(PackUriHelper.CreatePartUri(uri)).GetStream(FileMode.Open, FileAccess.Read);
        using var buffer = new MemoryStream();
        stream.CopyTo(buffer);
        return buffer.ToArray();
    }

    /// <summary>Kütüphanenin yazıp açık bıraktığı parça stream'ini baştan okur.</summary>
    private static byte[] BastanOku(Stream stream)
    {
        stream.Flush();
        stream.Position = 0;
        using var buffer = new MemoryStream();
        stream.CopyTo(buffer);
        return buffer.ToArray();
    }

    // Atlas imzayı doğrulamaz; paket özetinin SHA-256'sı ve zaman damgası imza bileşenine yazılır
    private static byte[] SahteImza(byte[] paketOzeti) =>
        SHA256.HashData(paketOzeti).Concat(BitConverter.GetBytes(DateTime.Now.Ticks)).ToArray();

    // Kütüphane bazı parça stream'lerini açık bırakıyor; Kapat'tan önce kapatılmazsa içerikleri yazılmayabilir
    private static readonly FieldInfo? PackageField =
        typeof(Paket).GetField("_package", BindingFlags.NonPublic | BindingFlags.Instance);

    private static void CloseInnerStreams(Paket paket)
    {
        if (PackageField?.GetValue(paket) is not Package package)
            return;

        foreach (var part in package.GetParts())
        {
            // Moduna göre (oluşturma/güncelleme) parça yeniden açılamayabilir; o parça kütüphanenin elinde kalır
            try { part.GetStream().Close(); }
            catch (Exception) { }
        }
    }

    private static CT_IletisimBilgisi Iletisim(string? adres, string? telefon, string? faks, string? ePosta, string? webAdresi) => new()
    {
        Adres = new TextType { Value = Dolu(adres) },
        Telefon = Dolu(telefon),
        Faks = Dolu(faks),
        EPosta = Dolu(ePosta),
        WebAdresi = Dolu(webAdresi)
    };

    private static string Dolu(string? value) => string.IsNullOrWhiteSpace(value) ? BosAlan : value.Trim();

    private static string Uzanti(string? dosyaAdi, string? mimeTuru)
    {
        var uzanti = Path.GetExtension(dosyaAdi ?? string.Empty);
        if (!string.IsNullOrEmpty(uzanti))
            return uzanti.ToLowerInvariant();
        return mimeTuru is null or "application/pdf" ? ".pdf" : ".dat";
    }

    private static ST_KodGuvenlikKodu GuvenlikKodu(string? kod) => kod?.ToUpperInvariant() switch
    {
        "YOK" => ST_KodGuvenlikKodu.YOK,
        "HZO" => ST_KodGuvenlikKodu.HZO,
        "OZL" => ST_KodGuvenlikKodu.OZL,
        "GZL" => ST_KodGuvenlikKodu.GZL,
        "CGZ" => ST_KodGuvenlikKodu.CGZ,
        _ => throw new InvalidOperationException($"Geçersiz gizlilik kodu: {kod}")
    };

    private static ST_KodDagitimTuru DagitimTuru(string? kod) => kod?.ToUpperInvariant() switch
    {
        "GRG" => ST_KodDagitimTuru.GRG,
        "BLG" => ST_KodDagitimTuru.BLG,
        _ => throw new InvalidOperationException($"Geçersiz dağıtım türü: {kod}")
    };

    private static ST_KodIvedilik Ivedilik(string? kod) => kod?.ToUpperInvariant() switch
    {
        "IVD" => ST_KodIvedilik.IVD,
        "CIV" => ST_KodIvedilik.CIV,
        "YLDRM" => ST_KodIvedilik.YLDRM,
        "GNL" => ST_KodIvedilik.GNL,
        _ => ST_KodIvedilik.NRM
    };

    private static ST_KodEkTuru EkTuru(string? kod) => kod?.ToUpperInvariant() switch
    {
        "DED" => ST_KodEkTuru.DED,
        "HRF" => ST_KodEkTuru.HRF,
        "FZK" => ST_KodEkTuru.FZK,
        _ => throw new InvalidOperationException($"Geçersiz ek türü: {kod}")
    };
}
