using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DijitalEvrakTakip.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class mg20 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentAllocations_IncomingDocuments_DocumentId",
                table: "DocumentAllocations");

            migrationBuilder.DropForeignKey(
                name: "FK_DocumentAllocations_IncomingDocuments_IncomingDocumentId",
                table: "DocumentAllocations");

            migrationBuilder.DropIndex(
                name: "IX_DocumentAllocations_DocumentId",
                table: "DocumentAllocations");

            migrationBuilder.DropColumn(
                name: "DocumentId",
                table: "DocumentAllocations");

            migrationBuilder.AlterColumn<Guid>(
                name: "IncomingDocumentId",
                table: "DocumentAllocations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentAllocations_IncomingDocuments_IncomingDocumentId",
                table: "DocumentAllocations",
                column: "IncomingDocumentId",
                principalTable: "IncomingDocuments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentAllocations_IncomingDocuments_IncomingDocumentId",
                table: "DocumentAllocations");

            migrationBuilder.AlterColumn<Guid>(
                name: "IncomingDocumentId",
                table: "DocumentAllocations",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "DocumentId",
                table: "DocumentAllocations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_DocumentAllocations_DocumentId",
                table: "DocumentAllocations",
                column: "DocumentId");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentAllocations_IncomingDocuments_DocumentId",
                table: "DocumentAllocations",
                column: "DocumentId",
                principalTable: "IncomingDocuments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentAllocations_IncomingDocuments_IncomingDocumentId",
                table: "DocumentAllocations",
                column: "IncomingDocumentId",
                principalTable: "IncomingDocuments",
                principalColumn: "Id");
        }
    }
}
