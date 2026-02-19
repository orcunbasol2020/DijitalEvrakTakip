using DijitalEvrakTakip.Domain.Abstractions;

namespace DijitalEvrakTakip.Domain.Entities;

public sealed class Role : Entity
{
    public string Name { get; set; }
    public bool IsActive { get; set; }

    public ICollection<UserRole> UserRoles { get; set; }
}
