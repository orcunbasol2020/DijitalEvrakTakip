namespace DijitalEvrakTakip.Domain.Dtos;

public sealed class ExternalInstitutionDto
{
    /// <summary>ComAgency.AgencyId; "Dilekçe / Talep" için -1</summary>
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    /// <summary>Bkz. ExternalInstitutionTypes: 1 misyon, 2 kurum, 3 dilekçe, 4 uluslararası kuruluş</summary>
    public int Type { get; set; }
    public string? Address { get; set; }
    public string? DetsisCode { get; set; }
    public int? ParentId { get; set; }
}
