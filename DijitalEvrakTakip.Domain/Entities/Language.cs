using DijitalEvrakTakip.Domain.Abstractions;

namespace DijitalEvrakTakip.Domain.Entities
{
    public class Language : Entity
    {
        public string? Name { get; set; }

        public bool? OcrSupport { get; set; }

        public bool? IsActive { get; set; }
    }
}
