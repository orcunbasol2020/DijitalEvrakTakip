/// <summary>
/// Dış kurum; yalnızca okunur. vw_ExternalInstitutions view'ından gelir: ComAgency (Dışişleri'nin kendi
/// birimleri ve Diplomatik Portal grup düğümleri hariç) + sabit "Dilekçe / Talep" göndereni.
/// Tür için bkz. ExternalInstitutionTypes.
/// </summary>
public class ExternalInstitution
{
    /// <summary>ComAgency.AgencyId; "Dilekçe / Talep" için ExternalInstitutionIds.Petition</summary>
    public int Id { get; set; }

    public int? ParentId { get; set; }
    public ExternalInstitution? Parent { get; set; }

    public ICollection<ExternalInstitution> Children { get; set; }
        = new List<ExternalInstitution>();

    public ICollection<ExternalUser> Users { get; set; }
        = new List<ExternalUser>();

    public int Type { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Address { get; set; }

    /// <summary>8 haneli DETSİS numarası (DETSİS kaynaklı kurumlarda AgencyId); misyonlarda ve dilekçede boş.</summary>
    public string? DetsisCode { get; set; }

    /// <summary>Kaynakta silinmiş ya da pasif. Eski evraklarda adı görünsün diye view'da kalır; listelerde süzülür.</summary>
    public bool IsDeleted { get; set; }
}
