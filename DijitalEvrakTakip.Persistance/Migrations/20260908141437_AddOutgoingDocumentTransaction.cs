using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DijitalEvrakTakip.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddOutgoingDocumentTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OutgoingDocumentTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OutgoingDocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: true),
                    CargoPostNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutgoingDocumentTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OutgoingDocumentTransactions_OutgoingDocuments_OutgoingDocumentId",
                        column: x => x.OutgoingDocumentId,
                        principalTable: "OutgoingDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OutgoingDocumentTransactions_OutgoingDocumentId",
                table: "OutgoingDocumentTransactions",
                column: "OutgoingDocumentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OutgoingDocumentTransactions");
        }
    }
}
