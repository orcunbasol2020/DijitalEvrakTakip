using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DijitalEvrakTakip.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddUserActiveIndexToAllocations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_OutgoingDocumentAllocations_UserId_IsActive_IsDeleted",
                table: "OutgoingDocumentAllocations",
                columns: new[] { "UserId", "IsActive", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentAllocations_UserId_IsActive_IsDeleted",
                table: "DocumentAllocations",
                columns: new[] { "UserId", "IsActive", "IsDeleted" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OutgoingDocumentAllocations_UserId_IsActive_IsDeleted",
                table: "OutgoingDocumentAllocations");

            migrationBuilder.DropIndex(
                name: "IX_DocumentAllocations_UserId_IsActive_IsDeleted",
                table: "DocumentAllocations");
        }
    }
}
