using DijitalEvrakTakip.Application.Services;
using DijitalEvrakTakip.Domain.Entities;

namespace DijitalEvrakTakip.Application.Features.ExternalInstitutionFeatures;

/// <summary>
/// Evrak cevaplarındaki ExternalInstitutionName alanını doldurur. Kurumlar ~160.000 kayıt olduğu için ön yüz
/// adları tüm kurum listesinden eşleyemez; ad, sonuçtaki kurumlar için tek ek sorguyla alınır.
/// </summary>
public static class ExternalInstitutionNames
{
    public static async Task FillAsync(
        this IExternalInstitutionService service,
        IEnumerable<IncomingDocument?> documents,
        CancellationToken cancellationToken)
    {
        var list = documents.Where(x => x?.ExternalInstitutionId != null).ToList();
        if (list.Count == 0)
            return;

        var names = await service.GetNamesAsync(list.Select(x => x!.ExternalInstitutionId!.Value), cancellationToken);
        foreach (var document in list)
            document!.ExternalInstitutionName = names.GetValueOrDefault(document.ExternalInstitutionId!.Value);
    }

    public static async Task FillAsync(
        this IExternalInstitutionService service,
        IEnumerable<OutgoingDocument?> documents,
        CancellationToken cancellationToken)
    {
        var list = documents.Where(x => x?.ExternalInstitutonId != null).ToList();
        if (list.Count == 0)
            return;

        var names = await service.GetNamesAsync(list.Select(x => x!.ExternalInstitutonId!.Value), cancellationToken);
        foreach (var document in list)
            document!.ExternalInstitutionName = names.GetValueOrDefault(document.ExternalInstitutonId!.Value);
    }
}
