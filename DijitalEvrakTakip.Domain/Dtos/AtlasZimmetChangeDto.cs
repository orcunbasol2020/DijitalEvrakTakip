namespace DijitalEvrakTakip.Domain.Dtos;

public sealed class AtlasZimmetChangeDto
{
    public Guid Id { get; set; }
    public string AtlasZimmetId { get; set; } = null!;
    public string AtlasDocumentNo { get; set; } = null!;
    public string? QrCode { get; set; }

    public string? FromUserSicilNo { get; set; }
    public string? FromUserName { get; set; }
    public string? FromUnitName { get; set; }

    public string? ToUserSicilNo { get; set; }
    public string? ToUserName { get; set; }
    public string? ToUnitName { get; set; }

    public string? ZimmetTuru { get; set; }
    public DateTime ZimmetTarihi { get; set; }
    public string? Description { get; set; }

    public int Status { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime? ProcessedDate { get; set; }

    public Guid? IncomingDocumentId { get; set; }
    public DateTime CreatedDate { get; set; }
}
