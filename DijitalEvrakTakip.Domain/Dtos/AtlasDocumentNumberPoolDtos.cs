namespace DijitalEvrakTakip.Domain.Dtos;

public sealed record AtlasDocumentNumberPoolStockDto(
    int Available,
    int Reserved,
    int Used,
    int Cancelled,
    // Ayardaki minimum stok; Available bunun altına düşünce havuz doldurulur
    int MinStock
);

public sealed record AtlasDocumentNumberRefillResultDto(
    // Doldurma çalıştı mı (kapalıysa, stok yeterliyse veya başka tur sürüyorsa false)
    bool Ran,
    int Requested,
    int Added,
    // Havuzda zaten bulunduğu için eklenmeyen numaralar
    int Duplicates,
    int AvailableAfter,
    string Message
);
