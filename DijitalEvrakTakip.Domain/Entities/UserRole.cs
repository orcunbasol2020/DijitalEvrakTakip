using DijitalEvrakTakip.Domain.Abstractions;

namespace DijitalEvrakTakip.Domain.Entities
{
    public sealed class UserRole : Entity
    {
        public Guid UserId { get; set; }     // FK
        public Guid RoleId { get; set; }     // FK

        public User User { get; set; }       // Navigation
        public Role Role { get; set; }       // Navigation
    }

}
