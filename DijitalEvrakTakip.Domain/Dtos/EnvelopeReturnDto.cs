namespace DijitalEvrakTakip.Domain.Dtos;

public sealed class EnvelopeReturnDto
{
    public Guid Id { get; set; }
    public string EnvelopeNo { get; set; } = default!;
    public string? UnitName { get; set; }
    public string? Address { get; set; }

    // Yeni alan
    public Guid? ExternalInstitutionId { get; set; }
    public string? ExternalInstitutionName { get; set; }
}