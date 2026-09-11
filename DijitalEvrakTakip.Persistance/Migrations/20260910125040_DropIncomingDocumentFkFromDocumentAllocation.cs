using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DijitalEvrakTakip.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class DropIncomingDocumentFkFromDocumentAllocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentAllocations_IncomingDocuments_IncomingDocumentId",
                table: "DocumentAllocations");

            migrationBuilder.DropIndex(
                name: "IX_DocumentAllocations_IncomingDocumentId",
                table: "DocumentAllocations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_DocumentAllocations_IncomingDocumentId",
                table: "DocumentAllocations",
                column: "IncomingDocumentId");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentAllocations_IncomingDocuments_IncomingDocumentId",
                table: "DocumentAllocations",
                column: "IncomingDocumentId",
                principalTable: "IncomingDocuments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
