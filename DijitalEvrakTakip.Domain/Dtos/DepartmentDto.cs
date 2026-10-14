namespace DijitalEvrakTakip.Domain.Dtos;

public sealed record DepartmentDto(
    int Id,
    string Name,
    string ShortName,
    int? ParentId = null,
    // ComDepartment.DepartmentTypeId, bkz. DepartmentTypes
    int? Type = null,
    // Makam seviyesi: 1 Bakan, 2 Bakan Yardımcılığı / makam birimleri, 4 GM, 5 GMY, 6 Daire Başkanlığı ...
    int? Level = null,
    // Aynı seviyede sıra; çoğunda boş
    int? SortOrder = null
);
