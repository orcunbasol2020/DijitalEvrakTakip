using DijitalEvrakTakip.Domain.Dtos;

namespace DijitalEvrakTakip.Application.Services;

/// <summary>
/// e-Yazışma standardına uygun EYP paketi oluşturur ve okur. Paket iki aşamada üretilir:
/// önce üstveri ve üst yazıyla paket açılır, sonra nihai üstveri ve imzayla tamamlanır.
/// Uygulama Cbddo.eYazisma kütüphanesini kullanan Infrastructure.Eyp.EypService'tedir.
/// </summary>
public interface IEypService
{
    /// <summary>
    /// Aşama 1: üstveri, dağıtım, ilgi, ek ve üst yazıyla paketi oluşturur.
    /// Pakete yazılan BelgeId ile paket içeriğini döner.
    /// </summary>
    /// <param name="fileContent">Üst yazı dosyası; zorunlu (dosyası olmayan evrakta fiziki gönderim PDF'i verilir).</param>
    Task<Tuple<string, byte[]>> EypOlusturAsync(EypMetadataDto metadata, byte[]? fileContent);

    /// <summary>
    /// Aşama 2: nihai üstveriyi (BelgeNo, Tarih, imzacı) yazar, paket özetini imzalar
    /// ve nihai özeti oluşturarak tamamlanmış paketi döner. Evrak kayıt birimi dışarıdan gelen evrakı
    /// paketlediği için imza sahtedir ve imzalayan "Sistem Yöneticisi"dir; Atlas imzayı doğrulamaz.
    /// </summary>
    /// <param name="eyp"><see cref="EypOlusturAsync"/> sonucu (BelgeId, paket içeriği).</param>
    Task<byte[]> EypTamamlaAsync(Tuple<string, byte[]> eyp, EypMetadataDto eypMetadataDto);

    /// <summary>
    /// Paketin üstverisini ve üst yazısını okur.
    /// </summary>
    Task<(EypMetadataDto Metadata, byte[]? FileContent)> ExtractEypPackageAsync(byte[] eypPackage);

    /// <summary>
    /// Paket açılabiliyor ve BelgeId içeriyorsa true döner.
    /// </summary>
    Task<bool> ValidateEypPackageAsync(byte[] eypPackage);
}
