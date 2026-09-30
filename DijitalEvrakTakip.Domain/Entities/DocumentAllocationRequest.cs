using DijitalEvrakTakip.Domain.Abstractions;

namespace DijitalEvrakTakip.Domain.Entities;

/// <summary>
/// Gelen evrakta kurum içi kullanıcıya yapılan Devir / Teslim talebi.
/// Alıcı onay verene kadar zimmet devredende kalır; onayda DocumentAllocations'a
/// Devir Alındı / Teslim Alındı statüsünde yeni aktif zimmet yazılır.
/// </summary>
public class DocumentAllocationRequest : Entity
{
    // Evrağın Id'si (FK değildir)
    public Guid IncomingDocumentId { get; set; }

    // Talep anındaki aktif zimmet; onayda hâlâ aktif değilse talep geçersiz sayılır
    public Guid? FromAllocationId { get; set; }

    // Talep anındaki zimmet sahibi (aktif zimmet yoksa boş)
    public Guid? FromUserId { get; set; }

    // Onay verecek kurum içi kullanıcı
    public Guid ToUserId { get; set; }

    // İstenen zimmet işlemi (AllocationStatusEnum: Devir / Teslim)
    public int RequestedAllocationStatus { get; set; }

    // Talep durumu (AllocationRequestStatusEnum)
    public int Status { get; set; }

    // İşlemi yapan kullanıcı (zimmet sahibinden farklı olabilir)
    public Guid RequestedByUserId { get; set; }

    public DateTime? RespondedDate { get; set; }

    // Onaylayan, reddeden veya iptal eden kullanıcı
    public Guid? RespondedUserId { get; set; }

    // Red / iptal gerekçesi
    public string? ResponseNote { get; set; }

    // Onayda oluşan DocumentAllocation
    public Guid? ResultAllocationId { get; set; }

    public int ReminderCount { get; set; }
    public DateTime? LastReminderDate { get; set; }
    public DateTime? NextReminderDate { get; set; }

    // Onay / red / iptal aynı anda gelirse ikincisi hata alır
    public byte[] RowVersion { get; set; } = null!;
}
