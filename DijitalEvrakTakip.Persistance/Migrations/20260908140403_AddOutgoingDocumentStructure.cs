using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DijitalEvrakTakip.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddOutgoingDocumentStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OutgoingDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QrCode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    OriginalDocumentNumber = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    SecurityDegree = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Type = table.Column<int>(type: "int", nullable: true),
                    LanguageId = table.Column<int>(type: "int", nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Content_Ocr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ExternalInstitutonId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ElectronicCopy = table.Column<bool>(type: "bit", nullable: true),
                    EbysTransfer = table.Column<bool>(type: "bit", nullable: true),
                    PageCount = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    DocumentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutgoingDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OutgoingDocuments_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OutgoingDocuments_ExternalInstitutions_ExternalInstitutonId",
                        column: x => x.ExternalInstitutonId,
                        principalTable: "ExternalInstitutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OutgoingDocuments_DepartmentId",
                table: "OutgoingDocuments",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_OutgoingDocuments_ExternalInstitutonId",
                table: "OutgoingDocuments",
                column: "ExternalInstitutonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OutgoingDocuments");
        }
    }
}
