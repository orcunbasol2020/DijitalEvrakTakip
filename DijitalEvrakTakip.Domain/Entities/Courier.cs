using DijitalEvrakTakip.Domain.Abstractions;

namespace DijitalEvrakTakip.Domain.Entities
{
    public class Courier : Entity
    {
        public string? UserId { get; set; }

        public string? DepartmentId { get; set; }

        public string? IsActive { get; set; }
    }
}
