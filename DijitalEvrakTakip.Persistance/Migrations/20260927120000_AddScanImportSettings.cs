using System;
using DijitalEvrakTakip.Persistance.Context;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DijitalEvrakTakip.Persistance.Migrations
{
    /// <summary>
    /// Taranan PDF içe aktarma ayarlarını AppSettings tablosuna ekler.
    /// Model değişikliği olmadığı için snapshot güncellenmez.
    /// </summary>
    [DbContext(typeof(AppDbContext))]
    [Migration("20260927120000_AddScanImportSettings")]
    public partial class AddScanImportSettings : Migration
    {
        private static readonly DateTime SeedDate = new DateTime(2026, 9, 27, 0, 0, 0, DateTimeKind.Utc);

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AppSettings",
                columns: new[] { "Id", "Key", "Value", "Description", "UpdatedByUserId", "IsDeleted", "CreatedDate", "UpdateDate" },
                columnTypes: new[] { "uniqueidentifier", "nvarchar(100)", "nvarchar(2000)", "nvarchar(500)", "uniqueidentifier", "bit", "datetime2", "datetime2" },
                values: new object[,]
                {
                    { Guid.Parse("6f1c2a9e-4b3d-4e0a-9c7f-1a2b3c4d5e05"), "ScanImportFolderPath", @"C:\EvrakTakip\belgeler\scanned", "Tarayıcının PDF bıraktığı klasör; buradaki PDF'ler taranmış belge olarak içe aktarılır", null, false, SeedDate, null },
                    { Guid.Parse("6f1c2a9e-4b3d-4e0a-9c7f-1a2b3c4d5e06"), "ScanImportEnabled", "true", "Taranan PDF'lerin otomatik içe aktarımı açık mı (true / false)", null, false, SeedDate, null },
                    { Guid.Parse("6f1c2a9e-4b3d-4e0a-9c7f-1a2b3c4d5e07"), "ScanImportIntervalSeconds", "30", "Klasörün kaç saniyede bir kontrol edileceği (en az 10)", null, false, SeedDate, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AppSettings",
                keyColumn: "Id",
                keyColumnType: "uniqueidentifier",
                keyValues: new object[]
                {
                    Guid.Parse("6f1c2a9e-4b3d-4e0a-9c7f-1a2b3c4d5e05"),
                    Guid.Parse("6f1c2a9e-4b3d-4e0a-9c7f-1a2b3c4d5e06"),
                    Guid.Parse("6f1c2a9e-4b3d-4e0a-9c7f-1a2b3c4d5e07")
                });
        }
    }
}
