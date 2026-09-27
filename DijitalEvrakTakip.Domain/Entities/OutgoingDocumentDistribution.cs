using System.Text.Json.Serialization;
using DijitalEvrakTakip.Domain.Abstractions;

namespace DijitalEvrakTakip.Domain.Entities
{
    /// <summary>
    /// Giden evrağın dağıtım listesi. Bir evrak birden fazla iç birime ve/veya
    /// dış kuruma gidebilir; her satır tek bir alıcıyı temsil eder.
    /// İç birim için DepartmentId, dış kurum için ExternalInstitutionId dolu olur.
    /// </summary>
    public class OutgoingDocumentDistribution : Entity
    {
        public Guid OutgoingDocumentId { get; set; }

        // OutgoingDocuments/GetAll evrakları Distributions ile birlikte döndürür;
        // EF fix-up bu geri referansı doldurduğundan JSON'a yazılırsa döngü oluşur.
        [JsonIgnore]
        public OutgoingDocument? OutgoingDocument { get; set; }

        public Guid? DepartmentId { get; set; }
        public Department? Department { get; set; }

        public Guid? ExternalInstitutionId { get; set; }
        public ExternalInstitution? ExternalInstitution { get; set; }

        // Bilgi/Gereği: true = Gereği, false = Bilgi
        public bool? ActionRequired { get; set; }

        // DeliveryMethodEnum: Elden / Kargo / EBYS
        public int? DeliveryMethod { get; set; }

        // Alıcıya gönderim tarihi
        public DateTime? SentDate { get; set; }

        // Alıcıya teslim tarihi
        public DateTime? DeliveryDate { get; set; }

        // Kargo ile gönderildiyse ilgili kargo kaydı
        public Guid? ShipmentId { get; set; }

        [JsonIgnore]
        public OutgoingDocumentShipment? Shipment { get; set; }

        public string? Notes { get; set; }
    }
}
