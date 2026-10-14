using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DijitalEvrakTakip.Persistance.Migrations
{
    /// <summary>
    /// vw_ExternalInstitutions'ta kurum adı baştaki / sondaki boşluk, sekme ve satır sonlarından arındırılır. Bazı DETSİS adları
    /// bunlarla başlıyor (ör. "    YASAMA", sekmeyle başlayan "TRABZON ..."); ada göre sıralı listelerde en başa düşüyorlardı. Ad kullanan bütün cevaplar
    /// (listeler, evraklardaki externalInstitutionName, dağıtım / gönderim) bu view'dan okuduğu için tek yerde düzelir.
    /// </summary>
    public partial class TrimExternalInstitutionNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Boşluk, sekme ve satır sonları (CHAR 32, 9, 10, 13)
            migrationBuilder.Sql(ViewSql("TRIM(CHAR(32) + CHAR(9) + CHAR(10) + CHAR(13) FROM a.Name)"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(ViewSql("a.Name"));
        }

        // 20261011175932_UseComAgencyForExternalInstitutions'taki tanım; yalnızca ad ifadesi değişir.
        // ALTER VIEW kendi batch'inde olmalı; idempotent betikte IF bloğuna sarıldığı için EXEC ile çalıştırılır.
        private static string ViewSql(string nameExpression) => @"
EXEC(N'ALTER VIEW vw_ExternalInstitutions AS
WITH Mfa AS (
    SELECT AgencyId, 0 AS Lvl FROM ComAgency WHERE AgencyId = 24305112
    UNION ALL
    SELECT c.AgencyId, Mfa.Lvl + 1 FROM ComAgency c JOIN Mfa ON c.ParentAgencyId = Mfa.AgencyId WHERE Mfa.Lvl < 20
)
SELECT a.AgencyId       AS Id,
       a.ParentAgencyId AS ParentId,
       " + nameExpression + @" AS Name,
       a.Address,
       CASE WHEN a.DataSourceId = 1 AND a.AgencyId BETWEEN 10000000 AND 99999999
            THEN CAST(a.AgencyId AS nvarchar(8)) END AS DetsisCode,
       CASE a.DataSourceId WHEN 2 THEN 1 WHEN 3 THEN 4 ELSE 2 END AS Type,
       CAST(CASE WHEN a.IsDeleted = 1 OR a.IsActive = 0 THEN 1 ELSE 0 END AS bit) AS IsDeleted
FROM ComAgency a
WHERE NOT EXISTS (SELECT 1 FROM Mfa WHERE Mfa.AgencyId = a.AgencyId)
  AND NOT (a.DataSourceId = 2 AND EXISTS (SELECT 1 FROM ComAgency c WHERE c.ParentAgencyId = a.AgencyId))
UNION ALL
SELECT -1, NULL, N''Dilekçe / Talep'', NULL, NULL, 3, CAST(0 AS bit)');";
    }
}
