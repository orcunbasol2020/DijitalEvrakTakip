using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DijitalEvrakTakip.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentAllocationRequestsAndNotifications : Migration
    {
        private static readonly DateTime SeedDate = new DateTime(2026, 9, 30, 0, 0, 0, DateTimeKind.Utc);

        private static readonly Guid[] SettingIds =
        {
            Guid.Parse("6f1c2a9e-4b3d-4e0a-9c7f-1a2b3c4d5e08"),
            Guid.Parse("6f1c2a9e-4b3d-4e0a-9c7f-1a2b3c4d5e09"),
            Guid.Parse("6f1c2a9e-4b3d-4e0a-9c7f-1a2b3c4d5e0a"),
            Guid.Parse("6f1c2a9e-4b3d-4e0a-9c7f-1a2b3c4d5e0b"),
            Guid.Parse("6f1c2a9e-4b3d-4e0a-9c7f-1a2b3c4d5e0c"),
            Guid.Parse("6f1c2a9e-4b3d-4e0a-9c7f-1a2b3c4d5e0d")
        };

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Zimmet onayı ve hatırlatma ayarları (model değişikliği değil, snapshot'a girmez)
            migrationBuilder.InsertData(
                table: "AppSettings",
                columns: new[] { "Id", "Key", "Value", "Description", "UpdatedByUserId", "IsDeleted", "CreatedDate", "UpdateDate" },
                columnTypes: new[] { "uniqueidentifier", "nvarchar(100)", "nvarchar(2000)", "nvarchar(500)", "uniqueidentifier", "bit", "datetime2", "datetime2" },
                values: new object[,]
                {
                    { SettingIds[0], "ZimmetApprovalRequired", "true", "Gelen evrakta kurum içi Devir / Teslim alıcının onayıyla mı gerçekleşsin (true / false)", null, false, SeedDate, null },
                    { SettingIds[1], "ZimmetReminderEnabled", "true", "Onay bekleyen zimmetler için hatırlatma bildirimi gönderilsin mi (true / false)", null, false, SeedDate, null },
                    { SettingIds[2], "ZimmetReminderIntervalHours", "4", "Onay bekleyen zimmet için kaç saatte bir hatırlatma yapılacağı (en az 1)", null, false, SeedDate, null },
                    { SettingIds[3], "ZimmetReminderEscalateAfter", "3", "Kaçıncı hatırlatmada devredene gecikme bildirimi gideceği (0 = gönderilmez)", null, false, SeedDate, null },
                    { SettingIds[4], "ZimmetReminderWorkStartHour", "8", "Hatırlatmaların gönderilmeye başlayacağı saat (hafta içi, sunucu saati)", null, false, SeedDate, null },
                    { SettingIds[5], "ZimmetReminderWorkEndHour", "18", "Hatırlatmaların durdurulacağı saat (hafta içi, sunucu saati)", null, false, SeedDate, null }
                });

            migrationBuilder.CreateTable(
                name: "DocumentAllocationRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IncomingDocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FromAllocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FromUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ToUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestedAllocationStatus = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    RequestedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RespondedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RespondedUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ResponseNote = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ResultAllocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReminderCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    LastReminderDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextReminderDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentAllocationRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    RelatedEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IncomingDocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ReadDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentAllocationRequests_Status_NextReminderDate",
                table: "DocumentAllocationRequests",
                columns: new[] { "Status", "NextReminderDate" });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentAllocationRequests_ToUserId_Status_IsDeleted",
                table: "DocumentAllocationRequests",
                columns: new[] { "ToUserId", "Status", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "UX_DocumentAllocationRequests_IncomingDocumentId_Pending",
                table: "DocumentAllocationRequests",
                column: "IncomingDocumentId",
                unique: true,
                filter: "[Status] = 1 AND [IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId_IsRead_IsDeleted",
                table: "Notifications",
                columns: new[] { "UserId", "IsRead", "IsDeleted" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentAllocationRequests");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DeleteData(
                table: "AppSettings",
                keyColumn: "Id",
                keyColumnType: "uniqueidentifier",
                keyValues: SettingIds.Cast<object>().ToArray());
        }
    }
}
