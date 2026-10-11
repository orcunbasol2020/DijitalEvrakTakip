using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DijitalEvrakTakip.Persistance.Migrations
{
    /// <summary>
    /// Kullanıcılar ComAuthorizationUser'dan, birimler ComDepartment'tan okunur (vw_Users, vw_Departments).
    /// Eski Users / Departments tabloları kaldırılır; evraklardaki birim kolonları Guid'den
    /// ComDepartment.DepartmentId'ye (int) çevrilir.
    ///
    /// uniqueidentifier -> int dönüşümü mümkün olmadığından kolonlar silinip yeniden eklenir: eski birim
    /// bağları taşınmaz. Evrak tablolarında birim bilgisi dolu kayıt varsa migration hata verip durur
    /// (önce eski birim -> ComDepartment eşlemesi yapılmalı). Zarflardaki (Envelopes) birim bilgisi boşaltılır.
    /// </summary>
    public partial class UseComTablesForUsersAndDepartments : Migration
    {
        private static readonly (string Table, string Column, bool Nullable, string Index)[] DepartmentColumns =
        {
            ("IncomingDocuments", "DepartmentId", true, "IX_IncomingDocuments_DepartmentId"),
            ("IncomingDocumentDistributions", "DepartmentId", false, "IX_IncomingDocumentDistributions_DepartmentId"),
            ("OutgoingDocuments", "DepartmentId", true, "IX_OutgoingDocuments_DepartmentId"),
            ("OutgoingDocumentDistributions", "DepartmentId", true, "IX_OutgoingDocumentDistributions_DepartmentId"),
            ("Envelopes", "DepartmentId", true, null),
            ("Envelopes", "TargetDepartmentId", true, null)
        };

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF EXISTS (SELECT 1 FROM IncomingDocuments WHERE DepartmentId IS NOT NULL)
   OR EXISTS (SELECT 1 FROM IncomingDocumentDistributions)
   OR EXISTS (SELECT 1 FROM OutgoingDocuments WHERE DepartmentId IS NOT NULL)
   OR EXISTS (SELECT 1 FROM OutgoingDocumentDistributions WHERE DepartmentId IS NOT NULL)
    THROW 50001, N'Evraklarda eski birim (Departments) bağları var; ComDepartment eşlemesi yapılmadan bu migration uygulanamaz.', 1;");

            migrationBuilder.DropForeignKey(
                name: "FK_IncomingDocumentDistributions_Departments_DepartmentId",
                table: "IncomingDocumentDistributions");

            migrationBuilder.DropForeignKey(
                name: "FK_IncomingDocuments_Departments_DepartmentId",
                table: "IncomingDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_OutgoingDocumentDistributions_Departments_DepartmentId",
                table: "OutgoingDocumentDistributions");

            migrationBuilder.DropForeignKey(
                name: "FK_OutgoingDocuments_Departments_DepartmentId",
                table: "OutgoingDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLoginLogs_Users_UserId",
                table: "UserLoginLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Users_UserId",
                table: "UserRoles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Departments");

            foreach (var (table, column, nullable, index) in DepartmentColumns)
            {
                if (index != null)
                    migrationBuilder.DropIndex(name: index, table: table);

                migrationBuilder.DropColumn(name: column, table: table);

                migrationBuilder.AddColumn<int>(
                    name: column,
                    table: table,
                    type: "int",
                    nullable: nullable,
                    defaultValue: nullable ? null : 0);

                if (index != null)
                    migrationBuilder.CreateIndex(name: index, table: table, column: column);
            }

            // CREATE VIEW kendi batch'inde olmalı; idempotent betikte IF bloğuna sarıldığı için EXEC ile çalıştırılır.
            migrationBuilder.Sql(@"
EXEC(N'CREATE VIEW vw_Departments AS
SELECT d.DepartmentId       AS Id,
       d.ParentDepartmentId AS ParentId,
       d.DefaultName        AS Name,
       d.DefaultShortName   AS ShortName,
       d.DepartmentTypeId   AS Type,
       d.IsDeleted
FROM ComDepartment d');");

            // UserName: NTLogonName'in 'HITIT\' öneki atılmış hali (AD sAMAccountName).
            // IsActive: kaynakta aktif, girişi engellenmemiş ve kurumdan ayrılmamış.
            migrationBuilder.Sql(@"
EXEC(N'CREATE VIEW vw_Users AS
SELECT u.UserId AS Id,
       CASE WHEN CHARINDEX(N''\'', u.NTLogonName) > 0
            THEN SUBSTRING(u.NTLogonName, CHARINDEX(N''\'', u.NTLogonName) + 1, 256)
            ELSE u.NTLogonName END AS UserName,
       u.Name,
       u.Surname,
       u.Email,
       a.DepartmentId,
       CAST(CASE WHEN u.IsActive = 1 AND u.LoginBlocked = 0
                      AND (u.LeaveDate IS NULL OR u.LeaveDate > GETDATE())
                 THEN 1 ELSE 0 END AS bit) AS IsActive,
       u.IsDeleted
FROM ComAuthorizationUser u
LEFT JOIN ComAgency a ON a.AgencyId = u.AgencyId');");
        }

        /// <inheritdoc />
        /// <remarks>
        /// Şemayı geri alır, veriyi değil: eski Users / Departments boş oluşur, evraklardaki birim bağları boş kalır.
        /// UserRoles / UserLoginLogs -> Users FK'leri geri eklenmez (kayıtlar artık Com kullanıcılarını gösterir).
        /// </remarks>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS vw_Users;");
            migrationBuilder.Sql("DROP VIEW IF EXISTS vw_Departments;");

            foreach (var (table, column, nullable, index) in DepartmentColumns)
            {
                if (index != null)
                    migrationBuilder.DropIndex(name: index, table: table);

                migrationBuilder.DropColumn(name: column, table: table);

                migrationBuilder.AddColumn<Guid>(
                    name: column,
                    table: table,
                    type: "uniqueidentifier",
                    nullable: nullable,
                    defaultValue: nullable ? null : Guid.Empty);

                if (index != null)
                    migrationBuilder.CreateIndex(name: index, table: table, column: column);
            }

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DisnetId = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ShortName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Departments_Departments_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AutType = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Surname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Departments_ParentId",
                table: "Departments",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DepartmentId",
                table: "Users",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_IncomingDocumentDistributions_Departments_DepartmentId",
                table: "IncomingDocumentDistributions",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IncomingDocuments_Departments_DepartmentId",
                table: "IncomingDocuments",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OutgoingDocumentDistributions_Departments_DepartmentId",
                table: "OutgoingDocumentDistributions",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OutgoingDocuments_Departments_DepartmentId",
                table: "OutgoingDocuments",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
