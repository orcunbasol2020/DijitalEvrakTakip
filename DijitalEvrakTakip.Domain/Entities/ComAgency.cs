namespace DijitalEvrakTakip.Domain.Entities
{
    /// <summary>
    /// Kurum/birim listesi. EGovernmentCmsCommon.dbo.ComAgency tablosunun birebir kopyası;
    /// anahtar ve kolonlar kaynakla aynı tutulur, veri oradan aktarılır.
    /// </summary>
    public class ComAgency
    {
        public int AgencyId { get; set; }
        public int? ParentAgencyId { get; set; }
        public int? DepartmentId { get; set; }

        public string AgencyCode { get; set; } = null!;
        public string? MainCode { get; set; }
        public string? DepartmentCode { get; set; }
        public string? SubAgencyCode1 { get; set; }
        public string? SubAgencyCode2 { get; set; }
        public string? SubAgencyCode3 { get; set; }
        public string? SubAgencyCode4 { get; set; }
        public string? DistributionCode1 { get; set; }
        public string? DistributionCode2 { get; set; }
        public string? DistributionCode3 { get; set; }
        public string? DistributionCode4 { get; set; }
        public bool HasSubAgency { get; set; }

        public string Name { get; set; } = null!;
        public string? Address { get; set; }
        public string? Email { get; set; }
        public string? Fax { get; set; }
        public string? Phone { get; set; }
        public string? TypeCode { get; set; }
        public string? WebAddress { get; set; }
        public string? KEPAddress { get; set; }

        public bool IsDeleted { get; set; }
        public int ECorrespondenceUsage { get; set; } = 1;
        public string? ECorrespondenceEndPoint { get; set; }
        public int DataSourceId { get; set; } = 1;
        public int? OldId { get; set; }
        public int IsCacheable { get; set; }
        public bool IsLocked { get; set; }
        public string? FullName { get; set; }
        public bool IsActive { get; set; } = true;
        public string? ECorrespondenceName { get; set; }
        public bool? CanSendECorrespondence { get; set; }
        public int ECorrespondenceType { get; set; }
        public int? ParentAdminAgencyId { get; set; }
        public string? PathFlaten { get; set; }
        public string? DistributionTitle { get; set; }
        public string? Position { get; set; }

        // Doluysa yazışma bu birim üzerinden yapılır; seçili birimle doğrudan yazışılamaz
        public int? CorrespondenceAgencyId { get; set; }
        public string? CorrespondenceAgencyText { get; set; }
        public bool? UseCorrespondenceAgencyIdAsAgencyId { get; set; }

        public int? KurumTip { get; set; }
        public string? CorrespondenceAgencyTextParent { get; set; }
        public string? DistributionTitleParent { get; set; }
        public long? Test { get; set; }
        public string? MersisNo { get; set; }
    }
}
