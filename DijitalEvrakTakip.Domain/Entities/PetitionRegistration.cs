using DijitalEvrakTakip.Domain.Abstractions;

namespace DijitalEvrakTakip.Domain.Entities
{
    public class PetitionRegistration : Entity
    {
        public string? QrCode { get; set; }

        public string? Name { get; set; }

        public string? Surname { get; set; }

        public string? TCKNo { get; set; }

        public bool? IsActive { get; set; }

        public string? CreateUserId { get; set; }
    }
}
