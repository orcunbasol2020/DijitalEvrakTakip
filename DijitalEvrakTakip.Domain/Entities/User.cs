using DijitalEvrakTakip.Domain.Abstractions;
using System.Security.Principal;

namespace DijitalEvrakTakip.Domain.Entities;

public sealed class User : Entity
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public bool AutType { get; set; }

    public Guid DepartmentId { get; set; }
    public Department Department { get; set; } // navigation property

    public bool IsActive { get; set; }

    public ICollection<UserRole> UserRoles { get; set; }
}
