using DijitalEvrakTakip.Domain.Abstractions;

namespace DijitalEvrakTakip.Domain.Entities
{
    /// <summary>
    /// EYP paketinin Atlas'a aktarımı için yapılan tek bir deneme (oluşturma + gönderim).
    /// CreatedDate denemenin zamanıdır.
    /// </summary>
    public class EypTransfer : Entity
    {
        public Guid EypPackageId { get; set; }

        // Pakete ait kaçıncı deneme olduğu (1'den başlar)
        public int AttemptNo { get; set; }

        public bool IsSuccess { get; set; }

        // Tekrar denemenin işe yaramayacağı hata (EYP üretilemedi, Atlas paketi reddetti, deneme sınırı doldu)
        public bool IsPermanentFailure { get; set; }

        public string? Error { get; set; }

        public string? AtlasReferenceId { get; set; }

        // Navigation
        public EypPackage? EypPackage { get; set; }
    }
}
