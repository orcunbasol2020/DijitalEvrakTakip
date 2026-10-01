using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DijitalEvrakTakip.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddDetsisCodeToExternalInstitution : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DetsisCode",
                table: "ExternalInstitutions",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExternalInstitutions_DetsisCode",
                table: "ExternalInstitutions",
                column: "DetsisCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ExternalInstitutions_DetsisCode",
                table: "ExternalInstitutions");

            migrationBuilder.DropColumn(
                name: "DetsisCode",
                table: "ExternalInstitutions");
        }
    }
}
