using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DijitalEvrakTakip.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddAppSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UpdatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSettings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppSettings_Key",
                table: "AppSettings",
                column: "Key",
                unique: true);

            // Seed: anahtarlar Domain.Constants.AppSettingKeys ile birebir aynı olmalı.
            migrationBuilder.InsertData(
                table: "AppSettings",
                columns: new[] { "Id", "Key", "Value", "Description", "UpdatedByUserId", "IsDeleted", "CreatedDate", "UpdateDate" },
                values: new object[,]
                {
                    { Guid.Parse("6f1c2a9e-4b3d-4e0a-9c7f-1a2b3c4d5e01"), "ApplicationName", "Dijital Evrak Takip", "Uygulamanın arayüzde görünen adı", null, false, SeedDate, null },
                    { Guid.Parse("6f1c2a9e-4b3d-4e0a-9c7f-1a2b3c4d5e02"), "SupportEmail", null, "Kullanıcıların sorun bildireceği destek e-posta adresi", null, false, SeedDate, null },
                    { Guid.Parse("6f1c2a9e-4b3d-4e0a-9c7f-1a2b3c4d5e03"), "SupportPhone", null, "Destek telefon numarası", null, false, SeedDate, null },
                    { Guid.Parse("6f1c2a9e-4b3d-4e0a-9c7f-1a2b3c4d5e04"), "AnnouncementMessage", null, "Tüm kullanıcılara gösterilecek duyuru veya bakım mesajı; boşsa gösterilmez", null, false, SeedDate, null }
                });
        }

        private static readonly DateTime SeedDate = new DateTime(2026, 9, 26, 0, 0, 0, DateTimeKind.Utc);

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppSettings");
        }
    }
}
