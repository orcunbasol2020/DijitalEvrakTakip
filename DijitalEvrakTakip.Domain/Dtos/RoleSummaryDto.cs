namespace DijitalEvrakTakip.Domain.Dtos;

public sealed record RoleSummaryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}
