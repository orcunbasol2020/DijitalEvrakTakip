namespace DijitalEvrakTakip.Domain.Dtos;

public sealed record AppSettingDto(
    Guid Id,
    string Key,
    string? Value,
    string? Description,
    Guid? UpdatedByUserId,
    DateTime? UpdateDate
);
