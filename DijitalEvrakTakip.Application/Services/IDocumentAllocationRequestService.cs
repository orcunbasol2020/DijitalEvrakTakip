using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Enums;

namespace DijitalEvrakTakip.Application.Services;

public interface IDocumentAllocationRequestService
{
    // AppSettings: ZimmetApprovalRequired (varsayılan açık)
    Task<bool> IsApprovalRequiredAsync(CancellationToken cancellationToken);

    Task<DocumentAllocationRequest?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<DocumentAllocationRequestDto?> GetPendingDtoByDocumentIdAsync(
        Guid incomingDocumentId,
        CancellationToken cancellationToken);

    // Talep, ZimmetTalebi transaction'ı ve alıcıya bildirim tek commit'te yazılır.
    // Aynı evrak için bu arada başka bekleyen talep açıldıysa false döner
    Task<bool> CreateAsync(
        DocumentAllocationRequest request,
        CancellationToken cancellationToken);

    // Evrağın aktif zimmeti talep anındakiyle aynı değilse talep Geçersiz yapılır ve Gecersiz döner.
    // notifySender false ise sonuç bildirimi gönderilmez (toplu işlemde NotifyBulkResultAsync ile toplanır)
    Task<AllocationRequestActionResultEnum> ApproveAsync(
        DocumentAllocationRequest request,
        bool notifySender,
        CancellationToken cancellationToken);

    Task<AllocationRequestActionResultEnum> RejectAsync(
        DocumentAllocationRequest request,
        string? note,
        bool notifySender,
        CancellationToken cancellationToken);

    Task<AllocationRequestActionResultEnum> CancelAsync(
        DocumentAllocationRequest request,
        Guid cancelledByUserId,
        string? note,
        bool notifyReceiver,
        CancellationToken cancellationToken);

    // Toplu onay / red / iptal sonrası her karşı tarafa tek özet bildirim.
    // type: ZimmetOnaylandi / ZimmetReddedildi (devredene) veya ZimmetTalebiIptal (alıcıya)
    Task NotifyBulkResultAsync(
        NotificationTypeEnum type,
        IList<DocumentAllocationRequest> requests,
        Guid actorUserId,
        string? note,
        CancellationToken cancellationToken);

    // Kullanıcının onayını bekleyen talepler
    Task<IList<DocumentAllocationRequestDto>> GetPendingByToUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken);

    // Kullanıcının devrettiği veya işlemini yaptığı talepler
    Task<IList<DocumentAllocationRequestDto>> GetSentByUserIdAsync(
        Guid userId,
        bool onlyPending,
        CancellationToken cancellationToken);

    Task<IList<DocumentAllocationRequestDto>> GetByDocumentIdAsync(
        Guid incomingDocumentId,
        CancellationToken cancellationToken);

    // Hatırlatma zamanı gelen talepler için bildirim üretir; üretilen hatırlatma sayısını döner
    Task<int> SendRemindersAsync(CancellationToken cancellationToken);
}
