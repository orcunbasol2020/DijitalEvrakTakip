using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DijitalEvrakTakip.Persistance.Migrations
{
    /// <summary>
    /// vw_Departments'ta üst birim (ParentId) silinmemiş en yakın üst birime bağlanır: ComDepartment'ta üst birim
    /// silinmişse zincirde yukarı çıkılır; üst birim hiç yoksa ya da silinmemiş bir ata bulunamazsa birim kök olur.
    /// Kaynakta üst birim için FK yok; geliştirme veritabanında 159 birimin üst birimi ComDepartment'ta yoktu,
    /// 20'sininki silinmişti ve bunlarla birlikte 344 birime köklerden ulaşılamıyordu (alt birim sorguları eksik
    /// kalıyordu). Ham değer ComDepartment.ParentDepartmentId'de durur.
    /// </summary>
    public partial class NormalizeDepartmentHierarchy : Migration
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
EXEC(N'ALTER VIEW vw_Departments AS
SELECT d.DepartmentId       AS Id,
       d.ParentDepartmentId AS ParentId,
       d.DefaultName        AS Name,
       d.DefaultShortName   AS ShortName,
       d.DepartmentTypeId   AS Type,
       d.IsDeleted
FROM ComDepartment d');");
        }
    }
}
