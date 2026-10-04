namespace DijitalEvrakTakip.Domain.Entities
{
    /// <summary>
    /// Departman listesi.
    /// EGovernmentCmsCommon.dbo.ComDepartment tablosunun birebir kopyası; veri oradan aktarılır.
    /// </summary>
    public class ComDepartment
    {
        public int DepartmentId { get; set; }
        public int? ParentDepartmentId { get; set; }
        public int CountryId { get; set; }
        public int? DepartmentTypeId { get; set; }
        public string? Code { get; set; }
        public string DefaultName { get; set; } = null!;
        public string? DefaultShortName { get; set; }
        public bool IsDeleted { get; set; }
        public int? TransferId { get; set; }
        public bool? IsExternal { get; set; }
        public bool? Trash { get; set; }
        public string? DutyArea { get; set; }
        public bool IsDomisticOrganization { get; set; }
        public int? DomisticOrganizationSortOrder { get; set; }
        public int? DepartmentLevel { get; set; }
        public int? RegionClass { get; set; }
        public string? CamLink { get; set; }
        public string? Email { get; set; }
        public int? CountyId { get; set; }
        public int? CityId { get; set; }
    }
}
