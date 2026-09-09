using DijitalEvrakTakip.Domain.Abstractions;
using DijitalEvrakTakip.Domain.Entities;

public sealed class ExternalUser : Entity
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public string IdentityNo { get; set; }
    public int UserType { get; set; }

    public Guid ExternalInstitutionId { get; set; }

    public ExternalInstitution ExternalInstitution { get; set; }

    public bool IsActive { get; set; }
}