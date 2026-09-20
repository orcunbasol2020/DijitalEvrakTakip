using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DijitalEvrakTakip.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AlterOutgoingDocumentLanguageIdToGuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // SQL Server has no int -> uniqueidentifier conversion, so the column must be
            // dropped and re-added. LanguageId values were never valid Language GUIDs
            // (the API only ever accepted ints here), so there is nothing worth preserving.
            migrationBuilder.DropColumn(
                name: "LanguageId",
                table: "OutgoingDocuments");

            migrationBuilder.AddColumn<Guid>(
                name: "LanguageId",
                table: "OutgoingDocuments",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LanguageId",
                table: "OutgoingDocuments");

            migrationBuilder.AddColumn<int>(
                name: "LanguageId",
                table: "OutgoingDocuments",
                type: "int",
                nullable: true);
        }
    }
}
