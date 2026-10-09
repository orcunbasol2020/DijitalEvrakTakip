using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DijitalEvrakTakip.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddComAuthorizationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ComAuthorizationUser",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PersonId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Surname = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AgencyId = table.Column<int>(type: "int", nullable: true),
                    TitleId = table.Column<int>(type: "int", nullable: true),
                    CountryId = table.Column<int>(type: "int", nullable: true),
                    NTLogonName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ConnectsFrom = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RegistrationNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LeaveDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatorUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModifierUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    AuthorizedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(96)", maxLength: 96, nullable: true),
                    SecureMobileNumber = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: true),
                    SecureMobileDynamicCode = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: true),
                    InfoNote = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DigitalSignatureStateId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ForceToChangePassword = table.Column<bool>(type: "bit", nullable: false),
                    LoginBlocked = table.Column<bool>(type: "bit", nullable: false),
                    StaffId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComAuthorizationUser", x => x.UserId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComAuthorizationUser_AgencyId",
                table: "ComAuthorizationUser",
                column: "AgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_ComAuthorizationUser_NTLogonName",
                table: "ComAuthorizationUser",
                column: "NTLogonName");

            migrationBuilder.CreateIndex(
                name: "IX_ComAuthorizationUser_PersonId",
                table: "ComAuthorizationUser",
                column: "PersonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComAuthorizationUser");
        }
    }
}
