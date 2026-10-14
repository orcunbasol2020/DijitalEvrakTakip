using DijitalEvrakTakip.Domain.Abstractions;

namespace DijitalEvrakTakip.Domain.Entities
{
    /// <summary>
    /// Gelen evrakın Atlas'a aktarılacak EYP paketi ve aktarım kuyruğundaki takip bilgisi.
    /// Evrak başına bir aktif (IsDeleted = false) kayıt bulunur. Paket bir kez üretilir, tekrar
    /// denemelerde aynı dosya gönderilir. Hatalı aktarım yeniden kuyruğa alınınca kayıt silinmiş
    /// işaretlenir ve evrakın güncel bilgileriyle yeni paket üretilir.
    /// </summary>
    public class EypPackage : Entity
    {
        public Guid DocumentId { get; set; }

        // EYP klasöründeki dosya adı; paket henüz üretilmediyse veya üretim hata verdiyse boş
        public string? FileName { get; set; }

        public long? FileSize { get; set; }

        // Paket dosyasının SHA-256 özeti (hex)
        public string? Sha256 { get; set; }

        // Evrakın dosyası yüklenmediği için üst yazı olarak "Fiziki olarak gönderilecektir" PDF'i kullanıldı
        public bool UsedPlaceholderContent { get; set; }

        // Oluşturma ve gönderim denemelerinin toplam sayısı
        public int TryCount { get; set; }

        public DateTime? LastAttemptAt { get; set; }

        // Geçici hatadan sonra bir sonraki denemenin en erken zamanı
        public DateTime? NextAttemptAt { get; set; }

        public string? LastError { get; set; }

        // Atlas'ın paketi kabul ettiğinde döndüğü referans
        public string? AtlasReferenceId { get; set; }

        public DateTime? TransferredAt { get; set; }

        // Navigation
        public IncomingDocument? Document { get; set; }

        public ICollection<EypTransfer> Transfers { get; set; } = new List<EypTransfer>();
    }
}
