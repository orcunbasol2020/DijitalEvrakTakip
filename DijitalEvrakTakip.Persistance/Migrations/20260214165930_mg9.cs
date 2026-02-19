using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DijitalEvrakTakip.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class mg9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentAllocations_IncomingDocuments_DocumentId1",
                table: "DocumentAllocations");

            migrationBuilder.RenameColumn(
                name: "DocumentId1",
                table: "DocumentAllocations",
                newName: "IncomingDocumentId");

            migrationBuilder.RenameIndex(
                name: "IX_DocumentAllocations_DocumentId1",
                table: "DocumentAllocations",
                newName: "IX_DocumentAllocations_IncomingDocumentId");

            migrationBuilder.AlterColumn<Guid>(
                name: "DocumentId",
                table: "DocumentTransactions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DocumentId",
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
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentAllocations_IncomingDocuments_IncomingDocumentId",
                table: "DocumentAllocations");

            migrationBuilder.RenameColumn(
                name: "IncomingDocumentId",
                table: "DocumentAllocations",
                newName: "DocumentId1");

            migrationBuilder.RenameIndex(
                name: "IX_DocumentAllocations_IncomingDocumentId",
                table: "DocumentAllocations",
                newName: "IX_DocumentAllocations_DocumentId1");

            migrationBuilder.AlterColumn<Guid>(
                name: "DocumentId",
                table: "DocumentTransactions",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "DocumentId",
                table: "DocumentAllocations",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentAllocations_IncomingDocuments_DocumentId1",
                table: "DocumentAllocations",
                column: "DocumentId1",
                principalTable: "IncomingDocuments",
                principalColumn: "Id");
        }
    }
}
