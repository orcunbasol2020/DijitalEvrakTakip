using DijitalEvrakTakip.Domain.Abstractions;

namespace DijitalEvrakTakip.Domain.Entities
{
    /// <summary>
    /// Giden evrağın kargo / posta ile gönderim kaydı. Bir paket birden fazla
    /// dağıtım satırını (aynı kuruma giden birkaç evrak) kapsayabilir; bu yüzden
    /// evraka değil, OutgoingDocumentDistribution satırlarına bağlanır.
    /// </summary>
    public class OutgoingDocumentShipment : Entity
    {
        // CargoCompanyEnum: PTT / Aras / Yurtiçi / MNG / Sürat / Kurye / Diğer
        public int CargoCompany { get; set; }

        // Kargo takip numarası
        public string TrackingNumber { get; set; } = default!;

        public DateTime SentDate { get; set; }

        // Kargoya veren kişi (zimmetli kullanıcı, User tablosuna ait, FK değildir)
        public Guid SentUserId { get; set; }

        public Guid? ExternalInstitutionId { get; set; }
        public ExternalInstitution? ExternalInstitution { get; set; }

        // Paket üzerindeki alıcı adı
        public string? RecipientName { get; set; }

        // ShipmentStatusEnum: KargoyaVerildi / Yolda / TeslimEdildi / İade
        public int Status { get; set; }

        public DateTime? DeliveredDate { get; set; }

        public decimal? Cost { get; set; }

        public string? Notes { get; set; }

        public ICollection<OutgoingDocumentDistribution>? Distributions { get; set; }
    }
}
