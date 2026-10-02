using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DijitalEvrakTakip.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddAtlasDocumentNumberPool : Migration
    {
        private static readonly DateTime SeedDate = new DateTime(2026, 10, 2, 0, 0, 0, DateTimeKind.Utc);

        private static readonly Guid[] SettingIds =
        {
            Guid.Parse("3b7d9e21-5a4c-4f8e-b1d2-7c6e5f4a3b01"),
            Guid.Parse("3b7d9e21-5a4c-4f8e-b1d2-7c6e5f4a3b02"),
            Guid.Parse("3b7d9e21-5a4c-4f8e-b1d2-7c6e5f4a3b03"),
            Guid.Parse("3b7d9e21-5a4c-4f8e-b1d2-7c6e5f4a3b04"),
            Guid.Parse("3b7d9e21-5a4c-4f8e-b1d2-7c6e5f4a3b05")
        };

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Atlas numara havuzu ayarları (model değişikliği değil, snapshot'a girmez)
            migrationBuilder.InsertData(
                table: "AppSettings",
                columns: new[] { "Id", "Key", "Value", "Description", "UpdatedByUserId", "IsDeleted", "CreatedDate", "UpdateDate" },
                columnTypes: new[] { "uniqueidentifier", "nvarchar(100)", "nvarchar(2000)", "nvarchar(500)", "uniqueidentifier", "bit", "datetime2", "datetime2" },
                values: new object[,]
                {
                    { SettingIds[0], "AtlasNumberPoolEnabled", "false", "Atlas evrak numarası havuzu arka planda otomatik doldurulsun mu (true / false)", null, false, SeedDate, null },
                    { SettingIds[1], "AtlasNumberPoolIntervalSeconds", "300", "Havuz stoğunun kaç saniyede bir kontrol edileceği (en az 30)", null, false, SeedDate, null },
                    { SettingIds[2], "AtlasNumberPoolMinStock", "100", "Boştaki numara sayısı bunun altına düşünce Atlas'tan yeni numara alınır", null, false, SeedDate, null },
                    { SettingIds[3], "AtlasNumberPoolBatchSize", "100", "Atlas'tan tek seferde alınacak numara adedi (en fazla 1000)", null, false, SeedDate, null },
                    { SettingIds[4], "AtlasNumberPoolEnforced", "false", "Gelen evrak yalnızca Atlas havuzundaki numarayla açılabilsin mi (true / false)", null, false, SeedDate, null }
                });

            migrationBuilder.CreateTable(
                name: "AtlasDocumentNumbers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QrCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AtlasReferenceId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ReservedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReservedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IncomingDocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UsedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancelledByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CancelledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancelReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AtlasDocumentNumbers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AtlasDocumentNumbers_IncomingDocuments_IncomingDocumentId",
                        column: x => x.IncomingDocumentId,
                        principalTable: "IncomingDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AtlasDocumentNumbers_IncomingDocumentId",
                table: "AtlasDocumentNumbers",
                column: "IncomingDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_AtlasDocumentNumbers_QrCode",
                table: "AtlasDocumentNumbers",
                column: "QrCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AtlasDocumentNumbers_Status_CreatedDate",
                table: "AtlasDocumentNumbers",
                columns: new[] { "Status", "CreatedDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AtlasDocumentNumbers");

            migrationBuilder.DeleteData(
                table: "AppSettings",
                keyColumn: "Id",
                keyColumnType: "uniqueidentifier",
                keyValues: SettingIds.Cast<object>().ToArray());
        }
    }
}
