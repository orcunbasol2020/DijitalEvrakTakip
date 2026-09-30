namespace DijitalEvrakTakip.Application.Features.DocumentAllocationRequestFeatures;

public static class AllocationRequestBulkLimits
{
    // Her talep ayrı commit'le işlendiği için istek süresi uzamasın diye sınır
    public const int MaxRequestCount = 500;
}
