using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DijitalEvrakTakip.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedUserIdToIncomingDocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedUserId",
                table: "IncomingDocuments",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedUserId",
                table: "IncomingDocuments");
        }
    }
}
