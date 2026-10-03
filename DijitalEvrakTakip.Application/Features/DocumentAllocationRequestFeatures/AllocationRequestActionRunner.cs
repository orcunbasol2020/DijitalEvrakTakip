using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Domain.Entities;
using DijitalEvrakTakip.Domain.Enums;

namespace DijitalEvrakTakip.Application.Features.DocumentAllocationRequestFeatures;

public enum AllocationRequestAction
{
    Approve,
    Reject,
    Cancel
}

/// <summary>
/// Onay / red / iptal için tekli ve toplu handler'ların ortak doğrulama, işlem ve mesaj mantığı.
/// Toplu işlemde karşı tarafa talep başına değil, kişi başına tek özet bildirim gider.
/// </summary>
public static class AllocationRequestActionRunner
{
    public static async Task<MessageResponse> RunSingleAsync(
        IDocumentAllocationRequestService service,
        AllocationRequestAction action,
        Guid requestId,
        string userIdValue,
        string? note,
        CancellationToken cancellationToken,
        bool hasDiscrepancy = false)
    {
        if (!Guid.TryParse(userIdValue, out var userId))
            return ToResponse(Result(requestId, AllocationRequestActionResultEnum.GecersizKullanici, action, null));

        var (result, _) = await RunAsync(service, action, requestId, userId, note, hasDiscrepancy, notify: true, cancellationToken);

        return ToResponse(result);
    }

    public static async Task<MessageResponse> RunBulkAsync(
        IDocumentAllocationRequestService service,
        AllocationRequestAction action,
        IList<Guid> requestIds,
        string userIdValue,
        string? note,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(userIdValue, out var userId))
            return new MessageResponse("Geçersiz UserId", new List<AllocationRequestActionResultDto>());

        var results = new List<AllocationRequestActionResultDto>();
        var succeeded = new List<DocumentAllocationRequest>();

        foreach (var requestId in requestIds.Distinct())
        {
            // Toplu onay şerhsizdir; şerh koyacak alıcı evrakı tek tek onaylar
            var (result, request) = await RunAsync(service, action, requestId, userId, note, hasDiscrepancy: false, notify: false, cancellationToken);

            results.Add(result);
            if (result.Result == (int)AllocationRequestActionResultEnum.Basarili && request is not null)
                succeeded.Add(request);
        }

        await service.NotifyBulkResultAsync(
            action switch
            {
                AllocationRequestAction.Approve => NotificationTypeEnum.ZimmetOnaylandi,
                AllocationRequestAction.Reject => NotificationTypeEnum.ZimmetReddedildi,
                _ => NotificationTypeEnum.ZimmetTalebiIptal
            },
            succeeded,
            userId,
            note,
            cancellationToken);

        var failed = results.Count - succeeded.Count;
        var message = failed == 0
            ? $"{succeeded.Count} zimmet talebi {GetVerb(action)}"
            : $"{succeeded.Count} zimmet talebi {GetVerb(action)}, {failed} talep işlenemedi";

        return new MessageResponse(message, results);
    }

    private static async Task<(AllocationRequestActionResultDto Result, DocumentAllocationRequest? Request)> RunAsync(
        IDocumentAllocationRequestService service,
        AllocationRequestAction action,
        Guid requestId,
        Guid userId,
        string? note,
        bool hasDiscrepancy,
        bool notify,
        CancellationToken cancellationToken)
    {
        var request = await service.GetByIdAsync(requestId, cancellationToken);

        var check = Check(request, userId, action);
        if (check is not null)
            return (Result(requestId, check.Value, action, request), request);

        var trimmedNote = string.IsNullOrWhiteSpace(note) ? null : note.Trim();

        var result = action switch
        {
            AllocationRequestAction.Approve => await service.ApproveAsync(request!, hasDiscrepancy, trimmedNote, notify, cancellationToken),
            AllocationRequestAction.Reject => await service.RejectAsync(request!, trimmedNote, notify, cancellationToken),
            _ => await service.CancelAsync(request!, userId, trimmedNote, notify, cancellationToken)
        };

        return (Result(requestId, result, action, request), request);
    }

    private static AllocationRequestActionResultEnum? Check(
        DocumentAllocationRequest? request,
        Guid userId,
        AllocationRequestAction action)
    {
        if (request is null)
            return AllocationRequestActionResultEnum.Bulunamadi;

        var authorized = action == AllocationRequestAction.Cancel
            ? request.FromUserId == userId || request.RequestedByUserId == userId
            : request.ToUserId == userId;

        if (!authorized)
            return AllocationRequestActionResultEnum.Yetkisiz;

        if (request.Status != (int)AllocationRequestStatusEnum.Beklemede)
            return AllocationRequestActionResultEnum.OnayBeklemiyor;

        return null;
    }

    private static AllocationRequestActionResultDto Result(
        Guid requestId,
        AllocationRequestActionResultEnum result,
        AllocationRequestAction action,
        DocumentAllocationRequest? request)
    {
        return new AllocationRequestActionResultDto
        {
            RequestId = requestId,
            Result = (int)result,
            Message = GetMessage(result, action, request)
        };
    }

    private static MessageResponse ToResponse(AllocationRequestActionResultDto result) =>
        new(result.Message, result);

    private static string GetVerb(AllocationRequestAction action) => action switch
    {
        AllocationRequestAction.Approve => "onaylandı",
        AllocationRequestAction.Reject => "reddedildi",
        _ => "iptal edildi"
    };

    private static string GetMessage(
        AllocationRequestActionResultEnum result,
        AllocationRequestAction action,
        DocumentAllocationRequest? request) => result switch
    {
        AllocationRequestActionResultEnum.Basarili => action switch
        {
            AllocationRequestAction.Approve => (request?.HasDiscrepancy == true ? "Zimmet şerhli olarak kabul edildi" : "Zimmet kabul edildi")
                + (request?.RequestedAllocationStatus == (int)AllocationStatusEnum.Teslim
                    ? "; evrak teslim alındı"
                    : "; evrak devir alındı"),
            AllocationRequestAction.Reject => "Zimmet reddedildi; evrak devreden kullanıcıda kaldı",
            _ => "Zimmet talebi iptal edildi"
        },
        AllocationRequestActionResultEnum.Gecersiz => "Talep beklerken evrağın zimmeti değiştiği için talep geçersiz sayıldı",
        AllocationRequestActionResultEnum.Cakisma => "Zimmet talebi başka bir işlemle güncellendi. Sayfayı yenileyip tekrar deneyin.",
        AllocationRequestActionResultEnum.Bulunamadi => "Zimmet talebi bulunamadı",
        AllocationRequestActionResultEnum.Yetkisiz => action switch
        {
            AllocationRequestAction.Approve => "Bu zimmet talebini yalnızca alıcı kullanıcı onaylayabilir",
            AllocationRequestAction.Reject => "Bu zimmet talebini yalnızca alıcı kullanıcı reddedebilir",
            _ => "Bu zimmet talebini yalnızca devreden veya işlemi yapan kullanıcı iptal edebilir"
        },
        AllocationRequestActionResultEnum.OnayBeklemiyor => "Zimmet talebi onay beklemiyor",
        _ => "Geçersiz UserId"
    };
}
