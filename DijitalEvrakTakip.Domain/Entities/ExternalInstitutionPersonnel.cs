using DijitalEvrakTakip.Domain.Abstractions;

namespace DijitalEvrakTakip.Domain.Entities
{
    public class ExternalInstitutionPersonnel : Entity
    {
        public string? Name { get; set; }

        public string? Surname { get; set; }

        public bool? IsActive { get; set; }
    }
}
