using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DijitalEvrakTakip.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class mg11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentTransactions_IncomingDocuments_DocumentId1",
                table: "DocumentTransactions");

            migrationBuilder.RenameColumn(
                name: "DocumentId1",
                table: "DocumentTransactions",
                newName: "IncomingDocumentId");

            migrationBuilder.RenameIndex(
                name: "IX_DocumentTransactions_DocumentId1",
                table: "DocumentTransactions",
                newName: "IX_DocumentTransactions_IncomingDocumentId");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentTransactions_IncomingDocuments_IncomingDocumentId",
                table: "DocumentTransactions",
                column: "IncomingDocumentId",
                principalTable: "IncomingDocuments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentTransactions_IncomingDocuments_IncomingDocumentId",
                table: "DocumentTransactions");

            migrationBuilder.RenameColumn(
                name: "IncomingDocumentId",
                table: "DocumentTransactions",
                newName: "DocumentId1");

            migrationBuilder.RenameIndex(
                name: "IX_DocumentTransactions_IncomingDocumentId",
                table: "DocumentTransactions",
                newName: "IX_DocumentTransactions_DocumentId1");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentTransactions_IncomingDocuments_DocumentId1",
                table: "DocumentTransactions",
                column: "DocumentId1",
                principalTable: "IncomingDocuments",
                principalColumn: "Id");
        }
    }
}
