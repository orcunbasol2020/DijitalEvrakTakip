using DijitalEvrakTakip.Domain.Abstractions;

namespace DijitalEvrakTakip.Domain.Entities
{
    public class CourierDuty : Entity
    {
        public string? CourierId { get; set; }

        public string? DepartmentId { get; set; }

        public int? DocumentCount { get; set; }

        public string? Note { get; set; }
    }
}
