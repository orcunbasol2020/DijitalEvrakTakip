using DijitalEvrakTakip.Domain.Abstractions;

namespace DijitalEvrakTakip.Domain.Entities
{
    public class DocumentType : Entity
    {
        public string? Name { get; set; }

        public bool? IsActive { get; set; }
    }
}
