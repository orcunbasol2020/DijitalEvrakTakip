using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DijitalEvrakTakip.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class mg8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DocumentAllocations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DocumentId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentAllocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentAllocations_IncomingDocuments_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "IncomingDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentAllocations_IncomingDocuments_DocumentId1",
                        column: x => x.DocumentId1,
                        principalTable: "IncomingDocuments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DocumentTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TransactionType = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DocumentId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentTransactions_IncomingDocuments_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "IncomingDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentTransactions_IncomingDocuments_DocumentId1",
                        column: x => x.DocumentId1,
                        principalTable: "IncomingDocuments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentAllocations_DocumentId",
                table: "DocumentAllocations",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentAllocations_DocumentId1",
                table: "DocumentAllocations",
                column: "DocumentId1");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTransactions_DocumentId",
                table: "DocumentTransactions",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTransactions_DocumentId1",
                table: "DocumentTransactions",
                column: "DocumentId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentAllocations");

            migrationBuilder.DropTable(
                name: "DocumentTransactions");
        }
    }
}
