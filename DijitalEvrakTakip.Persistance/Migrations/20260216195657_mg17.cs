using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DijitalEvrakTakip.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class mg17 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "IncomingDocuments",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ExternalInstitutionId",
                table: "IncomingDocuments",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DepartmentId",
                table: "IncomingDocuments",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_IncomingDocuments_DepartmentId",
                table: "IncomingDocuments",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_IncomingDocuments_ExternalInstitutionId",
                table: "IncomingDocuments",
                column: "ExternalInstitutionId");

            migrationBuilder.AddForeignKey(
                name: "FK_IncomingDocuments_Departments_DepartmentId",
                table: "IncomingDocuments",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IncomingDocuments_ExternalInstitutions_ExternalInstitutionId",
                table: "IncomingDocuments",
                column: "ExternalInstitutionId",
                principalTable: "ExternalInstitutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IncomingDocuments_Departments_DepartmentId",
                table: "IncomingDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_IncomingDocuments_ExternalInstitutions_ExternalInstitutionId",
                table: "IncomingDocuments");

            migrationBuilder.DropIndex(
                name: "IX_IncomingDocuments_DepartmentId",
                table: "IncomingDocuments");

            migrationBuilder.DropIndex(
                name: "IX_IncomingDocuments_ExternalInstitutionId",
                table: "IncomingDocuments");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "IncomingDocuments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 450,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ExternalInstitutionId",
                table: "IncomingDocuments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DepartmentId",
                table: "IncomingDocuments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
