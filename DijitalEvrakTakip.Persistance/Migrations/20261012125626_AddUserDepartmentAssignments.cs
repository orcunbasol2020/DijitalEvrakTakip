using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DijitalEvrakTakip.Persistance.Migrations
{
    /// <summary>
    /// Kullanıcının DISNET'teki birimi için UserDepartmentAssignments tablosu. Kaynak sistemde çoğu kişi yalnızca üst birimde
    /// (ör. BTGY) görünür; bu tabloda kayıt varsa vw_Users.DepartmentId onu, yoksa eskisi gibi ComAgency'deki birimi verir.
    /// Kullanıcıya göre birim süzen her sorgu (Users/GetAll?departmentId=..., birim altındaki kullanıcılar ...) view'dan
    /// okuduğu için tek yerde değişir.
    /// </summary>
    public partial class AddUserDepartmentAssignments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserDepartmentAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDepartmentAssignments", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserDepartmentAssignments_DepartmentId",
                table: "UserDepartmentAssignments",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDepartmentAssignments_UserId",
                table: "UserDepartmentAssignments",
                column: "UserId",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.Sql(ViewSql(
                "COALESCE(da.DepartmentId, a.DepartmentId)",
                "\r\nLEFT JOIN UserDepartmentAssignments da ON da.UserId = u.UserId AND da.IsDeleted = 0"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(ViewSql("a.DepartmentId", ""));

            migrationBuilder.DropTable(
                name: "UserDepartmentAssignments");
        }

        // 20261011173320_UseComTablesForUsersAndDepartments'taki tanım; yalnızca birim ifadesi ve ek join değişir.
        // ALTER VIEW kendi batch'inde olmalı; idempotent betikte IF bloğuna sarıldığı için EXEC ile çalıştırılır.
        private static string ViewSql(string departmentExpression, string extraJoin) => @"
EXEC(N'ALTER VIEW vw_Users AS
SELECT u.UserId AS Id,
       CASE WHEN CHARINDEX(N''\'', u.NTLogonName) > 0
            THEN SUBSTRING(u.NTLogonName, CHARINDEX(N''\'', u.NTLogonName) + 1, 256)
            ELSE u.NTLogonName END AS UserName,
       u.Name,
       u.Surname,
       u.Email,
       " + departmentExpression + @" AS DepartmentId,
       CAST(CASE WHEN u.IsActive = 1 AND u.LoginBlocked = 0
                      AND (u.LeaveDate IS NULL OR u.LeaveDate > GETDATE())
                 THEN 1 ELSE 0 END AS bit) AS IsActive,
       u.IsDeleted
FROM ComAuthorizationUser u
LEFT JOIN ComAgency a ON a.AgencyId = u.AgencyId" + extraJoin + @"');";
    }
}
