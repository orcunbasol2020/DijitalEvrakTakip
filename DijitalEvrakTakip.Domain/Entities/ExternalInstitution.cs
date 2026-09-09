using DijitalEvrakTakip.Domain.Abstractions;

public class ExternalInstitution : Entity
{
    public Guid? ParentId { get; set; }
    public ExternalInstitution? Parent { get; set; }

    public ICollection<ExternalInstitution> Children { get; set; }
        = new List<ExternalInstitution>();

    public ICollection<ExternalUser> Users { get; set; }
        = new List<ExternalUser>();

    public int Type { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;
}