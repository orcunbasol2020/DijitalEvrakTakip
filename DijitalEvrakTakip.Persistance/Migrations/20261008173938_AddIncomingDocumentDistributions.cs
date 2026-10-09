using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DijitalEvrakTakip.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddIncomingDocumentDistributions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IncomingDocumentDistributions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IncomingDocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActionRequired = table.Column<bool>(type: "bit", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncomingDocumentDistributions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncomingDocumentDistributions_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IncomingDocumentDistributions_IncomingDocuments_IncomingDocumentId",
                        column: x => x.IncomingDocumentId,
                        principalTable: "IncomingDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IncomingDocumentDistributions_DepartmentId",
                table: "IncomingDocumentDistributions",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_IncomingDocumentDistributions_IncomingDocumentId",
                table: "IncomingDocumentDistributions",
                column: "IncomingDocumentId");

            // Mevcut evrakların tekil "Nereye" birimi, evrağın Bilgi/Gereği değeriyle tek alıcı olarak taşınır
            migrationBuilder.Sql(@"
INSERT INTO IncomingDocumentDistributions (Id, IncomingDocumentId, DepartmentId, ActionRequired, Notes, IsDeleted, CreatedDate, UpdateDate)
SELECT NEWID(), d.Id, d.DepartmentId, d.ActionRequired, NULL, 0, GETUTCDATE(), NULL
FROM IncomingDocuments d
WHERE d.DepartmentId IS NOT NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IncomingDocumentDistributions");
        }
    }
}
