using System.Text.Json.Serialization;
using DijitalEvrakTakip.Domain.Abstractions;

namespace DijitalEvrakTakip.Domain.Entities
{
    /// <summary>
    /// Gelen evrağın "Nereye" listesi. Bir evrak birden fazla iç birime gelebilir;
    /// her satır tek bir alıcı birimi temsil eder ve Bilgi/Gereği alıcı bazında tutulur.
    /// </summary>
    public class IncomingDocumentDistribution : Entity
    {
        public Guid IncomingDocumentId { get; set; }

        // IncomingDocuments/GetAll evrakları Distributions ile birlikte döndürür;
        // EF fix-up bu geri referansı doldurduğundan JSON'a yazılırsa döngü oluşur.
        [JsonIgnore]
        public IncomingDocument? IncomingDocument { get; set; }

        public int DepartmentId { get; set; }
        public Department? Department { get; set; }

        // Bilgi/Gereği: true = Gereği, false = Bilgi
        public bool? ActionRequired { get; set; }

        public string? Notes { get; set; }
    }
}
