namespace DijitalEvrakTakip.Domain.Dtos;

/// <summary>
/// Gelen evrak zimmet onay talebi; evrak ve kişi adlarıyla birlikte.
/// </summary>
public sealed class DocumentAllocationRequestDto
{
    public Guid Id { get; set; }
    public Guid IncomingDocumentId { get; set; }
    public string? OrginalNo { get; set; }
    public string? QrCode { get; set; }
    public string? DocumentName { get; set; }
    public string? Subject { get; set; }
    public DateTime? DocumentDate { get; set; }

    // Devredenin beyanı; alıcı onay ekranında kontrol eder (evrakın güncel değerleri)
    public int? PageCount { get; set; }
    public bool? HasAttachment { get; set; }
    public string? AttachmentDescription { get; set; }

    public Guid? FromUserId { get; set; }
    public string FromUserFullName { get; set; } = string.Empty;
    public Guid ToUserId { get; set; }
    public string ToUserFullName { get; set; } = string.Empty;
    public Guid RequestedByUserId { get; set; }
    public string RequestedByFullName { get; set; } = string.Empty;

    // AllocationStatusEnum: 2 (Devir) / 3 (Teslim)
    public int RequestedAllocationStatus { get; set; }

    // AllocationRequestStatusEnum
    public int Status { get; set; }

    public string? ResponseNote { get; set; }
    public bool HasDiscrepancy { get; set; }
    public DateTime? RespondedDate { get; set; }
    public Guid? ResultAllocationId { get; set; }
    public int ReminderCount { get; set; }
    public DateTime? LastReminderDate { get; set; }
    public DateTime CreatedDate { get; set; }
}
