namespace DijitalEvrakTakip.Domain.Dtos;

public sealed record DepartmentDto(
    Guid Id,
    string Name,
    string ShortName,
    Guid? ParentId = null,
    int? DisnetId = null
);
