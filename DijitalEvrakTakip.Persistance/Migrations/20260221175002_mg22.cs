using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DijitalEvrakTakip.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class mg22 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "DocumentAllocations",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentAllocations_Users_UserId",
                table: "DocumentAllocations");

            migrationBuilder.DropIndex(
                name: "IX_DocumentAllocations_UserId",
                table: "DocumentAllocations");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "DocumentAllocations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");
        }
    }
}
