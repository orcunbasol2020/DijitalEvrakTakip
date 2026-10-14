using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DijitalEvrakTakip.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddAtlasTransferQueue : Migration
    {
        private static readonly DateTime SeedDate = new DateTime(2026, 10, 5, 0, 0, 0, DateTimeKind.Utc);

        private static readonly Guid[] SettingIds =
        {
            Guid.Parse("8c2f4a61-3d7b-4e95-a0c8-5b1e9d7f2a01"),
            Guid.Parse("8c2f4a61-3d7b-4e95-a0c8-5b1e9d7f2a02"),
            Guid.Parse("8c2f4a61-3d7b-4e95-a0c8-5b1e9d7f2a03"),
            Guid.Parse("8c2f4a61-3d7b-4e95-a0c8-5b1e9d7f2a04"),
            Guid.Parse("8c2f4a61-3d7b-4e95-a0c8-5b1e9d7f2a05"),
            Guid.Parse("8c2f4a61-3d7b-4e95-a0c8-5b1e9d7f2a06")
        };

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Atlas aktarım kuyruğu ayarları (model değişikliği değil, snapshot'a girmez).
            // KKK boşken aktarım turu çalışmaz; evraklar kuyrukta bekler.
            migrationBuilder.InsertData(
                table: "AppSettings",
                columns: new[] { "Id", "Key", "Value", "Description", "UpdatedByUserId", "IsDeleted", "CreatedDate", "UpdateDate" },
                columnTypes: new[] { "uniqueidentifier", "nvarchar(100)", "nvarchar(2000)", "nvarchar(500)", "uniqueidentifier", "bit", "datetime2", "datetime2" },
                values: new object[,]
                {
                    { SettingIds[0], "AtlasTransferEnabled", "false", "Yayınlanan evraklar arka planda otomatik olarak Atlas'a aktarılsın mı (true / false)", null, false, SeedDate, null },
                    { SettingIds[1], "AtlasTransferIntervalSeconds", "60", "Aktarım kuyruğunun kaç saniyede bir kontrol edileceği (en az 10)", null, false, SeedDate, null },
                    { SettingIds[2], "AtlasTransferBatchSize", "10", "Bir turda kuyruktan alınacak en fazla evrak sayısı (1 - 100)", null, false, SeedDate, null },
                    { SettingIds[3], "AtlasTransferMaxTryCount", "5", "Gönderim bu kadar denemede başarısız olursa evrak Hatalı olur (1 - 20)", null, false, SeedDate, null },
                    { SettingIds[4], "AtlasEypRecipientKkk", "", "EYP dağıtım listesine yazılan, evrakı alan kurumun KKK (DETSİS) kodu", null, false, SeedDate, null },
                    { SettingIds[5], "AtlasEypRecipientName", "T.C. Dışişleri Bakanlığı", "EYP dağıtım listesine yazılan, evrakı alan kurumun adı", null, false, SeedDate, null }
                });

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmissionUpdatedAt",
                table: "IncomingDocuments",
                type: "datetime2",
                nullable: true);

            // Daha önce yayınlanmış evraklar kuyrukta kayıt sırasıyla işlensin. EXEC ile çalıştırılır:
            // betik tek parça çalıştırıldığında yeni kolon derleme anında henüz yoktur.
            migrationBuilder.Sql(
                "EXEC(N'UPDATE IncomingDocuments SET SubmissionUpdatedAt = COALESCE(UpdateDate, CreatedDate) " +
                "WHERE SubmissionStatus IS NOT NULL AND SubmissionUpdatedAt IS NULL')");

            migrationBuilder.CreateTable(
                name: "EypPackages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FileSize = table.Column<long>(type: "bigint", nullable: true),
                    Sha256 = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UsedPlaceholderContent = table.Column<bool>(type: "bit", nullable: false),
                    TryCount = table.Column<int>(type: "int", nullable: false),
                    LastAttemptAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextAttemptAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastError = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    AtlasReferenceId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TransferredAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EypPackages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EypPackages_IncomingDocuments_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "IncomingDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EypTransfers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EypPackageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttemptNo = table.Column<int>(type: "int", nullable: false),
                    IsSuccess = table.Column<bool>(type: "bit", nullable: false),
                    IsPermanentFailure = table.Column<bool>(type: "bit", nullable: false),
                    Error = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    AtlasReferenceId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EypTransfers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EypTransfers_EypPackages_EypPackageId",
                        column: x => x.EypPackageId,
                        principalTable: "EypPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IncomingDocuments_SubmissionStatus_SubmissionUpdatedAt",
                table: "IncomingDocuments",
                columns: new[] { "SubmissionStatus", "SubmissionUpdatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_EypPackages_DocumentId",
                table: "EypPackages",
                column: "DocumentId",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_EypTransfers_EypPackageId",
                table: "EypTransfers",
                column: "EypPackageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EypTransfers");

            migrationBuilder.DropTable(
                name: "EypPackages");

            migrationBuilder.DropIndex(
                name: "IX_IncomingDocuments_SubmissionStatus_SubmissionUpdatedAt",
                table: "IncomingDocuments");

            migrationBuilder.DropColumn(
                name: "SubmissionUpdatedAt",
                table: "IncomingDocuments");

            migrationBuilder.DeleteData(
                table: "AppSettings",
                keyColumn: "Id",
                keyColumnType: "uniqueidentifier",
                keyValues: SettingIds.Cast<object>().ToArray());
        }
    }
}
