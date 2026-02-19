using DijitalEvrakTakip.Domain.Abstractions;

namespace DijitalEvrakTakip.Domain.Entities
{
    public class CourierDutyDocumentList : Entity
    {
        public Guid? CourierDutyId { get; set; }

        public Guid? DocumentId { get; set; }

        // Navigation
        public CourierDuty? CourierDuty { get; set; }

        public IncomingDocument? Document { get; set; }
    }
}
