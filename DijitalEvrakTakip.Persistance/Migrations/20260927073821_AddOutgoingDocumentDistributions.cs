using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DijitalEvrakTakip.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddOutgoingDocumentDistributions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OutgoingDocumentDistributions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OutgoingDocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ExternalInstitutionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ActionRequired = table.Column<bool>(type: "bit", nullable: true),
                    SentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CargoPostNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutgoingDocumentDistributions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OutgoingDocumentDistributions_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OutgoingDocumentDistributions_ExternalInstitutions_ExternalInstitutionId",
                        column: x => x.ExternalInstitutionId,
                        principalTable: "ExternalInstitutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OutgoingDocumentDistributions_OutgoingDocuments_OutgoingDocumentId",
                        column: x => x.OutgoingDocumentId,
                        principalTable: "OutgoingDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OutgoingDocumentDistributions_DepartmentId",
                table: "OutgoingDocumentDistributions",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_OutgoingDocumentDistributions_ExternalInstitutionId",
                table: "OutgoingDocumentDistributions",
                column: "ExternalInstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_OutgoingDocumentDistributions_OutgoingDocumentId",
                table: "OutgoingDocumentDistributions",
                column: "OutgoingDocumentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OutgoingDocumentDistributions");
        }
    }
}
