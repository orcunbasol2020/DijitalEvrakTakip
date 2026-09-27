using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DijitalEvrakTakip.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddOutgoingDocumentShipments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CargoPostNumber",
                table: "OutgoingDocumentTransactions");

            migrationBuilder.DropColumn(
                name: "CargoPostNumber",
                table: "OutgoingDocumentDistributions");

            migrationBuilder.AddColumn<Guid>(
                name: "ShipmentId",
                table: "OutgoingDocumentTransactions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeliveryMethod",
                table: "OutgoingDocumentDistributions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ShipmentId",
                table: "OutgoingDocumentDistributions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OutgoingDocumentShipments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CargoCompany = table.Column<int>(type: "int", nullable: false),
                    TrackingNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SentUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExternalInstitutionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RecipientName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    DeliveredDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Cost = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutgoingDocumentShipments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OutgoingDocumentShipments_ExternalInstitutions_ExternalInstitutionId",
                        column: x => x.ExternalInstitutionId,
                        principalTable: "ExternalInstitutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OutgoingDocumentDistributions_ShipmentId",
                table: "OutgoingDocumentDistributions",
                column: "ShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_OutgoingDocumentShipments_ExternalInstitutionId",
                table: "OutgoingDocumentShipments",
                column: "ExternalInstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_OutgoingDocumentShipments_TrackingNumber",
                table: "OutgoingDocumentShipments",
                column: "TrackingNumber");

            migrationBuilder.AddForeignKey(
                name: "FK_OutgoingDocumentDistributions_OutgoingDocumentShipments_ShipmentId",
                table: "OutgoingDocumentDistributions",
                column: "ShipmentId",
                principalTable: "OutgoingDocumentShipments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OutgoingDocumentDistributions_OutgoingDocumentShipments_ShipmentId",
                table: "OutgoingDocumentDistributions");

            migrationBuilder.DropTable(
                name: "OutgoingDocumentShipments");

            migrationBuilder.DropIndex(
                name: "IX_OutgoingDocumentDistributions_ShipmentId",
                table: "OutgoingDocumentDistributions");

            migrationBuilder.DropColumn(
                name: "ShipmentId",
                table: "OutgoingDocumentTransactions");

            migrationBuilder.DropColumn(
                name: "DeliveryMethod",
                table: "OutgoingDocumentDistributions");

            migrationBuilder.DropColumn(
                name: "ShipmentId",
                table: "OutgoingDocumentDistributions");

            migrationBuilder.AddColumn<string>(
                name: "CargoPostNumber",
                table: "OutgoingDocumentTransactions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CargoPostNumber",
                table: "OutgoingDocumentDistributions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
