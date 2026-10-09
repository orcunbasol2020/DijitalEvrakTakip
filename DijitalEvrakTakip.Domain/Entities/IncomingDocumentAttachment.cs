using System.Text.Json.Serialization;
using DijitalEvrakTakip.Domain.Abstractions;

namespace DijitalEvrakTakip.Domain.Entities
{
    /// <summary>
    /// Gelen evrağın elektronik eki (PDF). Atlas'a giden EYP paketine DED türünde, içeriğiyle eklenir.
    /// Fiziki ekler bu tabloda tutulmaz; IncomingDocument.AttachmentDescription metniyle tanımlanır.
    /// </summary>
    public class IncomingDocumentAttachment : Entity
    {
        public Guid IncomingDocumentId { get; set; }

        [JsonIgnore]
        public IncomingDocument? IncomingDocument { get; set; }

        // EYP ek listesinde görünen ad
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        // EYP'deki sıra: OrderNo, sonra yüklenme zamanına göre; fiziki ek listenin sonuna eklenir
        public int OrderNo { get; set; }

        // Kullanıcının yüklediği dosyanın adı
        public string OriginalFileName { get; set; } = null!;

        // Ek klasörüne göre yolu: {IncomingDocumentId}/{guid}.pdf
        public string StoredFileName { get; set; } = null!;

        public long FileSize { get; set; }

        public string? UploadedUserId { get; set; }
    }
}
