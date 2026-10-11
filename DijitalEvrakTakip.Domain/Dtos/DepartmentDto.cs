namespace DijitalEvrakTakip.Domain.Dtos;

public sealed record DepartmentDto(
    int Id,
    string Name,
    string ShortName,
    int? ParentId = null,
    // ComDepartment.DepartmentTypeId, bkz. DepartmentTypes
    int? Type = null
);
