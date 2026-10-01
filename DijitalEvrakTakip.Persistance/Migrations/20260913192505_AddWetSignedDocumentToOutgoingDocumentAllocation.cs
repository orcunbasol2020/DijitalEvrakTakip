using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DijitalEvrakTakip.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddWetSignedDocumentToOutgoingDocumentAllocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WetSignedDocumentFileName",
                table: "OutgoingDocumentAllocations",
                type: "nvarchar(260)",
                maxLength: 260,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WetSignedDocumentPath",
                table: "OutgoingDocumentAllocations",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "WetSignedDocumentUploadDate",
                table: "OutgoingDocumentAllocations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "WetSignedDocumentUploadedUserId",
                table: "OutgoingDocumentAllocations",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WetSignedDocumentFileName",
                table: "OutgoingDocumentAllocations");

            migrationBuilder.DropColumn(
                name: "WetSignedDocumentPath",
                table: "OutgoingDocumentAllocations");

            migrationBuilder.DropColumn(
                name: "WetSignedDocumentUploadDate",
                table: "OutgoingDocumentAllocations");

            migrationBuilder.DropColumn(
                name: "WetSignedDocumentUploadedUserId",
                table: "OutgoingDocumentAllocations");
        }
    }
}
