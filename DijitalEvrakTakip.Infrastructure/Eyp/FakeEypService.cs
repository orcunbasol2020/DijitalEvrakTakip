using System.IO.Compression;
using System.Text.Json;
using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;

namespace DijitalEvrakTakip.Infrastructure.Eyp;

/// <summary>
/// e-Yazışma kütüphanesi (Cbddo.eYazisma) projeye eklenene kadar kullanılan sahte EYP servisi.
/// Standart EYP üretmez: üstveriyi JSON olarak ve üst yazıyı bir zip içine koyar. Kuyruğun uçtan uca
/// denenebilmesi içindir; gerçek adaptör yazılınca DI kaydı değiştirilir.
/// Gerçek adaptörün kontrolleri (dağıtım listesi zorunlu, tamamlamada BelgeNo ve Tarih zorunlu) korunur.
/// </summary>
public sealed class FakeEypService : IEypService
{
    private const string UstveriEntry = "Ustveri.json";
    private const string NihaiUstveriEntry = "NihaiUstveri.json";
    private const string UstYaziFolder = "UstYazi/";

    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public Task<Tuple<string, byte[]>> EypOlusturAsync(EypMetadataDto metadata, byte[]? fileContent)
    {
        if (fileContent is null || fileContent.Length == 0)
            throw new NotSupportedException("Sahte EYP servisi metinden üst yazı üretmez; üst yazı dosyası verilmelidir.");

        if (metadata.DagitimListesi.Count == 0)
            throw new InvalidOperationException($"Dağıtım listesi boş, EYP oluşturulamaz: {metadata.DocumentId}");

        var belgeId = Guid.TryParse(metadata.DocumentId, out var parsed) ? parsed : Guid.NewGuid();

        using var buffer = new MemoryStream();
        using (var zip = new ZipArchive(buffer, ZipArchiveMode.Create, leaveOpen: true))
        {
            WriteJson(zip, UstveriEntry, metadata);
            WriteBytes(zip, UstYaziFolder + (metadata.FileName ?? "ustyazi.pdf"), fileContent);
        }

        return Task.FromResult(Tuple.Create(belgeId.ToString(), buffer.ToArray()));
    }

    public Task<byte[]> EypTamamlaAsync(Tuple<string, byte[]> eyp, EypMetadataDto eypMetadataDto)
    {
        if (string.IsNullOrWhiteSpace(eypMetadataDto.BelgeNo) || eypMetadataDto.DocumentDate is null)
            throw new InvalidOperationException("EYP tamamlanamadı: belge numarası ve tarih zorunludur.");

        using var buffer = new MemoryStream();
        buffer.Write(eyp.Item2);

        using (var zip = new ZipArchive(buffer, ZipArchiveMode.Update, leaveOpen: true))
        {
            WriteJson(zip, NihaiUstveriEntry, new
            {
                eypMetadataDto.BelgeNo,
                Tarih = eypMetadataDto.DocumentDate,
                Imza = "Sahte EYP: imzasız"
            });
        }

        return Task.FromResult(buffer.ToArray());
    }

    public Task<(EypMetadataDto Metadata, byte[]? FileContent)> ExtractEypPackageAsync(byte[] eypPackage)
    {
        using var zip = new ZipArchive(new MemoryStream(eypPackage), ZipArchiveMode.Read);

        var ustveri = zip.GetEntry(UstveriEntry)
            ?? throw new InvalidOperationException("EYP paketi ayrıştırılamadı: üstveri yok.");

        EypMetadataDto metadata;
        using (var stream = ustveri.Open())
            metadata = JsonSerializer.Deserialize<EypMetadataDto>(stream, JsonOptions)
                ?? throw new InvalidOperationException("EYP paketi ayrıştırılamadı: üstveri okunamadı.");

        byte[]? fileContent = null;
        var ustYazi = zip.Entries.FirstOrDefault(x => x.FullName.StartsWith(UstYaziFolder, StringComparison.Ordinal));
        if (ustYazi is not null)
        {
            using var stream = ustYazi.Open();
            using var content = new MemoryStream();
            stream.CopyTo(content);
            fileContent = content.ToArray();
            metadata.FileSize = fileContent.Length;
        }

        return Task.FromResult<(EypMetadataDto, byte[]?)>((metadata, fileContent));
    }

    public async Task<bool> ValidateEypPackageAsync(byte[] eypPackage)
    {
        try
        {
            var (metadata, _) = await ExtractEypPackageAsync(eypPackage);
            return !string.IsNullOrEmpty(metadata.DocumentId);
        }
        catch (Exception)
        {
            return false;
        }
    }

    private static void WriteJson(ZipArchive zip, string entryName, object value)
    {
        zip.GetEntry(entryName)?.Delete();
        using var stream = zip.CreateEntry(entryName).Open();
        JsonSerializer.Serialize(stream, value, JsonOptions);
    }

    private static void WriteBytes(ZipArchive zip, string entryName, byte[] content)
    {
        using var stream = zip.CreateEntry(entryName).Open();
        stream.Write(content);
    }
}
