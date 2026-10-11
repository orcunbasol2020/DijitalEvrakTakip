using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DijitalEvrakTakip.Persistance.Migrations
{
    /// <summary>
    /// vw_Departments'a makam seviyesi (Level = ComDepartment.DepartmentLevel) ve seviyedeki sıra
    /// (SortOrder = DomisticOrganizationSortOrder) eklenir; birim listesi bunlara göre sıralanır.
    ///
    /// Ayrıca kaynakta üst birimi boş olan 2. seviye merkez birimler (Bakan Yardımcılıkları, Özel Kalem, Sözcülük,
    /// Özel Müşavirlikler, Müsteşar ...) "Bakan" biriminin (ComDepartment 11082872) altına bağlanır; kaynakta yalnızca
    /// 2 Bakan Yardımcılığı Bakan'a bağlıydı. Bu kaynakta olmayan, bizim tarafta yapılan bir yorumdur. Bakan birimi
    /// silinir ya da bulunamazsa bu birimler eskisi gibi kök kalır.
    /// </summary>
    public partial class AddDepartmentLevelAndMinisterRoot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ALTER VIEW kendi batch'inde olmalı; idempotent betikte IF bloğuna sarıldığı için EXEC ile çalıştırılır.
            migrationBuilder.Sql(@"
EXEC(N'ALTER VIEW vw_Departments AS
WITH Up AS (
    SELECT d.DepartmentId AS Id, d.ParentDepartmentId AS AncestorId, 1 AS Lvl
    FROM ComDepartment d
    UNION ALL
    SELECT u.Id, p.ParentDepartmentId, u.Lvl + 1
    FROM Up u JOIN ComDepartment p ON p.DepartmentId = u.AncestorId
    WHERE p.IsDeleted = 1 AND u.Lvl < 20
), Eff AS (
    SELECT u.Id, u.AncestorId, ROW_NUMBER() OVER (PARTITION BY u.Id ORDER BY u.Lvl) AS Rn
    FROM Up u JOIN ComDepartment a ON a.DepartmentId = u.AncestorId AND a.IsDeleted = 0
    WHERE u.AncestorId <> u.Id
), Minister AS (
    SELECT DepartmentId FROM ComDepartment WHERE DepartmentId = 11082872 AND IsDeleted = 0
)
SELECT d.DepartmentId       AS Id,
       CASE WHEN e.AncestorId IS NULL AND d.IsDeleted = 0 AND d.DepartmentTypeId = 30006 AND d.DepartmentLevel = 2
            THEN (SELECT DepartmentId FROM Minister)
            ELSE e.AncestorId END AS ParentId,
       d.DefaultName        AS Name,
       d.DefaultShortName   AS ShortName,
       d.DepartmentTypeId   AS Type,
       d.DepartmentLevel    AS Level,
       d.DomisticOrganizationSortOrder AS SortOrder,
       d.IsDeleted
FROM ComDepartment d
LEFT JOIN Eff e ON e.Id = d.DepartmentId AND e.Rn = 1');");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
EXEC(N'ALTER VIEW vw_Departments AS
WITH Up AS (
    SELECT d.DepartmentId AS Id, d.ParentDepartmentId AS AncestorId, 1 AS Lvl
    FROM ComDepartment d
    UNION ALL
    SELECT u.Id, p.ParentDepartmentId, u.Lvl + 1
    FROM Up u JOIN ComDepartment p ON p.DepartmentId = u.AncestorId
    WHERE p.IsDeleted = 1 AND u.Lvl < 20
), Eff AS (
    SELECT u.Id, u.AncestorId, ROW_NUMBER() OVER (PARTITION BY u.Id ORDER BY u.Lvl) AS Rn
    FROM Up u JOIN ComDepartment a ON a.DepartmentId = u.AncestorId AND a.IsDeleted = 0
    WHERE u.AncestorId <> u.Id
)
SELECT d.DepartmentId       AS Id,
       e.AncestorId         AS ParentId,
       d.DefaultName        AS Name,
       d.DefaultShortName   AS ShortName,
       d.DepartmentTypeId   AS Type,
       d.IsDeleted
FROM ComDepartment d
LEFT JOIN Eff e ON e.Id = d.DepartmentId AND e.Rn = 1');");
        }
    }
}
