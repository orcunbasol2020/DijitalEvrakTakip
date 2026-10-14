using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DijitalEvrakTakip.Persistance.Migrations
{
    /// <summary>
    /// vw_Users.TitleName, kaynakta silinmiş unvanlarda da dolu gelir. Kaynak kayıt kişiyi hâlâ o unvanla gösteriyor;
    /// filtre yüzünden 31 aktif kullanıcının (9 silinmiş unvan, ör. Ticaret Müşaviri, Müşavir) unvanı boş dönüyordu.
    /// </summary>
    public partial class ShowDeletedTitlesInUsersView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(ViewSql(""));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(ViewSql(" AND t.IsDeleted = 0"));
        }

        // 20261013083310_AddComTitle'daki tanım; yalnızca ComTitle join'indeki silinmişlik koşulu değişir.
        // ALTER VIEW kendi batch'inde olmalı; idempotent betikte IF bloğuna sarıldığı için EXEC ile çalıştırılır.
        private static string ViewSql(string titleCondition) => @"
EXEC(N'ALTER VIEW vw_Users AS
SELECT u.UserId AS Id,
       CASE WHEN CHARINDEX(N''\'', u.NTLogonName) > 0
            THEN SUBSTRING(u.NTLogonName, CHARINDEX(N''\'', u.NTLogonName) + 1, 256)
            ELSE u.NTLogonName END AS UserName,
       u.Name,
       u.Surname,
       u.Email,
       COALESCE(da.DepartmentId, a.DepartmentId) AS DepartmentId,
       t.DefaultName AS TitleName,
       CAST(CASE WHEN u.IsActive = 1 AND u.LoginBlocked = 0
                      AND (u.LeaveDate IS NULL OR u.LeaveDate > GETDATE())
                 THEN 1 ELSE 0 END AS bit) AS IsActive,
       u.IsDeleted
FROM ComAuthorizationUser u
LEFT JOIN ComAgency a ON a.AgencyId = u.AgencyId
LEFT JOIN UserDepartmentAssignments da ON da.UserId = u.UserId AND da.IsDeleted = 0
LEFT JOIN ComTitle t ON t.TitleId = u.TitleId" + titleCondition + @"');";
    }
}
