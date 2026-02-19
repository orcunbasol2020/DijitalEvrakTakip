using DijitalEvrakTakip.Domain.Abstractions;

namespace DijitalEvrakTakip.Domain.Entities
{
    public class TransactionType : Entity
    {
        public string Name { get; set; }

        public bool? IsActive { get; set; }
    }
}
