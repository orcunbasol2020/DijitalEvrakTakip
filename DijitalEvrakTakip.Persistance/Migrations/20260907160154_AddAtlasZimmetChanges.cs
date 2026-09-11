using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DijitalEvrakTakip.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddAtlasZimmetChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AtlasZimmetChanges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AtlasZimmetId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AtlasDocumentNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    QrCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FromUserSicilNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FromUserName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FromUnitName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ToUserSicilNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ToUserName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ToUnitName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ZimmetTuru = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ZimmetTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RawPayload = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ProcessedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IncomingDocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AtlasZimmetChanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AtlasZimmetChanges_IncomingDocuments_IncomingDocumentId",
                        column: x => x.IncomingDocumentId,
                        principalTable: "IncomingDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AtlasZimmetChanges_AtlasZimmetId",
                table: "AtlasZimmetChanges",
                column: "AtlasZimmetId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AtlasZimmetChanges_IncomingDocumentId",
                table: "AtlasZimmetChanges",
                column: "IncomingDocumentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AtlasZimmetChanges");
        }
    }
}
