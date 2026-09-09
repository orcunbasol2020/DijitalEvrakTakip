namespace DijitalEvrakTakip.Domain.Dtos;

public sealed class ExternalInstitutionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Type { get; set; }
    public string Address { get; set; } = string.Empty;
}
