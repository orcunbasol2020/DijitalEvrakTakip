using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DijitalEvrakTakip.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class DropUserFkAddUserTypeToDocumentAllocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentAllocations_Users_UserId",
                table: "DocumentAllocations");

            migrationBuilder.DropIndex(
                name: "IX_DocumentAllocations_UserId",
                table: "DocumentAllocations");

            migrationBuilder.AddColumn<int>(
                name: "UserType",
                table: "DocumentAllocations",
                type: "int",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserType",
                table: "DocumentAllocations");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentAllocations_UserId",
                table: "DocumentAllocations",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentAllocations_Users_UserId",
                table: "DocumentAllocations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
