namespace DijitalEvrakTakip.Domain.Entities
{
    /// <summary>
    /// Unvan listesi. EGovernmentCmsCommon.dbo.ComTitle'ın kopyası olacak; kaynak şema gelene kadar geçici yapı,
    /// kolonlar ComDepartment adlandırmasına uyduruldu. TitleId kaynaktaki Id ile aynıdır (ComAuthorizationUser.TitleId),
    /// isimler şimdilik DISNET dökümlerinden eşleştirilerek doldurulur.
    /// </summary>
    public class ComTitle
    {
        public int TitleId { get; set; }
        public string DefaultName { get; set; } = null!;
        public string? DefaultShortName { get; set; }
        public bool IsDeleted { get; set; }
    }
}
