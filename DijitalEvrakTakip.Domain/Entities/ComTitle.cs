namespace DijitalEvrakTakip.Domain.Entities
{
    /// <summary>
    /// Unvan listesi. EGovernmentCmsCommon.dbo.ComTitle tablosunun birebir kopyası; veri oradan aktarılır.
    /// TitleId, ComAuthorizationUser.TitleId ile eşleşir.
    /// </summary>
    public class ComTitle
    {
        public int TitleId { get; set; }
        public string DefaultName { get; set; } = null!;
        public string? DefaultShortName { get; set; }
        public short? DefaultSortOrder { get; set; }
        public string? Code { get; set; }
        public bool IsDeleted { get; set; }
        public int? TransferId { get; set; }
        public bool? IsExternal { get; set; }
        public bool? Trash { get; set; }
    }
}
