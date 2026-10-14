using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DijitalEvrakTakip.Persistance.Migrations
{
    /// <summary>
    /// Kaynak şema gelene kadar geçici ComTitle tablosu ve vw_Users.TitleName. TitleId kaynaktaki Id'dir
    /// (ComAuthorizationUser.TitleId), isimler DISNET dökümlerinden eşleştirilerek ayrıca doldurulur; tablo boşken
    /// TitleName boş döner.
    /// </summary>
    public partial class AddComTitle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ComTitle",
                columns: table => new
                {
                    TitleId = table.Column<int>(type: "int", nullable: false),
                    DefaultName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DefaultShortName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComTitle", x => x.TitleId);
                });

            migrationBuilder.Sql(ViewSql(withTitle: true));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(ViewSql(withTitle: false));

            migrationBuilder.DropTable(
                name: "ComTitle");
        }

        // 20261012125626_AddUserDepartmentAssignments'taki tanım; yalnızca unvan kolonu ve join'i eklenir.
        // ALTER VIEW kendi batch'inde olmalı; idempotent betikte IF bloğuna sarıldığı için EXEC ile çalıştırılır.
        private static string ViewSql(bool withTitle) => @"
EXEC(N'ALTER VIEW vw_Users AS
SELECT u.UserId AS Id,
       CASE WHEN CHARINDEX(N''\'', u.NTLogonName) > 0
            THEN SUBSTRING(u.NTLogonName, CHARINDEX(N''\'', u.NTLogonName) + 1, 256)
            ELSE u.NTLogonName END AS UserName,
       u.Name,
       u.Surname,
       u.Email,
       COALESCE(da.DepartmentId, a.DepartmentId) AS DepartmentId," + (withTitle ? @"
       t.DefaultName AS TitleName," : "") + @"
       CAST(CASE WHEN u.IsActive = 1 AND u.LoginBlocked = 0
                      AND (u.LeaveDate IS NULL OR u.LeaveDate > GETDATE())
                 THEN 1 ELSE 0 END AS bit) AS IsActive,
       u.IsDeleted
FROM ComAuthorizationUser u
LEFT JOIN ComAgency a ON a.AgencyId = u.AgencyId
LEFT JOIN UserDepartmentAssignments da ON da.UserId = u.UserId AND da.IsDeleted = 0" + (withTitle ? @"
LEFT JOIN ComTitle t ON t.TitleId = u.TitleId AND t.IsDeleted = 0" : "") + @"');";
    }
}
