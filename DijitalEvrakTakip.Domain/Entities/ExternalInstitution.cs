using DijitalEvrakTakip.Domain.Abstractions;

namespace DijitalEvrakTakip.Domain.Entities
{
    public class ExternalInstitution : Entity
    {
        public Guid? ParentId { get; set; }
        public ExternalInstitution? Parent { get; set; }

        public ICollection<ExternalInstitution> Children { get; set; }
            = new List<ExternalInstitution>();

        public int? Type { get; set; }

        public string Name { get; set; } = string.Empty;
    }

}
