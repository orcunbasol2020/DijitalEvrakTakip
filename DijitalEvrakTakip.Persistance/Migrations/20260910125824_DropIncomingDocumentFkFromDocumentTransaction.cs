using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DijitalEvrakTakip.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class DropIncomingDocumentFkFromDocumentTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentTransactions_IncomingDocuments_DocumentId",
                table: "DocumentTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_DocumentTransactions_IncomingDocuments_IncomingDocumentId",
                table: "DocumentTransactions");

            migrationBuilder.DropIndex(
                name: "IX_DocumentTransactions_DocumentId",
                table: "DocumentTransactions");

            migrationBuilder.DropIndex(
                name: "IX_DocumentTransactions_IncomingDocumentId",
                table: "DocumentTransactions");

            migrationBuilder.DropColumn(
                name: "IncomingDocumentId",
                table: "DocumentTransactions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "IncomingDocumentId",
                table: "DocumentTransactions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTransactions_DocumentId",
                table: "DocumentTransactions",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTransactions_IncomingDocumentId",
                table: "DocumentTransactions",
                column: "IncomingDocumentId");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentTransactions_IncomingDocuments_DocumentId",
                table: "DocumentTransactions",
                column: "DocumentId",
                principalTable: "IncomingDocuments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentTransactions_IncomingDocuments_IncomingDocumentId",
                table: "DocumentTransactions",
                column: "IncomingDocumentId",
                principalTable: "IncomingDocuments",
                principalColumn: "Id");
        }
    }
}
