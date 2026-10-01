namespace DijitalEvrakTakip.Domain.Dtos;

public sealed record LanguageDto(
    Guid Id,
    string? Name,
    bool? OcrSupport,
    bool? IsActive
);
