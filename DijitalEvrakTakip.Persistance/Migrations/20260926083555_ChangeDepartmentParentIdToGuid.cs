using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DijitalEvrakTakip.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDepartmentParentIdToGuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // SQL Server int -> uniqueidentifier ALTER COLUMN dönüşümüne izin vermez.
            // Mevcut veride ParentId tamamen NULL olduğu için kolon silinip yeniden oluşturulur.
            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Departments");

            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                table: "Departments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departments_ParentId",
                table: "Departments",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Departments_ParentId",
                table: "Departments",
                column: "ParentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Departments_ParentId",
                table: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_Departments_ParentId",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Departments");

            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "Departments",
                type: "int",
                nullable: true);
        }
    }
}
