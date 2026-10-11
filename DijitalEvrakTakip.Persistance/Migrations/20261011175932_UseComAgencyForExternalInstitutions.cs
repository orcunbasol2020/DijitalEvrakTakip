using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DijitalEvrakTakip.Persistance.Migrations
{
    /// <summary>
    /// Dış kurumlar ComAgency'den okunur (vw_ExternalInstitutions). Eski ExternalInstitutions tablosu kaldırılır;
    /// evraklardaki, zarflardaki, gönderimlerdeki ve dış kurum personelindeki kurum kolonları Guid'den
    /// ComAgency.AgencyId'ye (int) çevrilir.
    ///
    /// uniqueidentifier -> int dönüşümü mümkün olmadığından kolonlar silinip yeniden eklenir: eski kurum bağları
    /// taşınmaz. Evraklarda kurum bilgisi dolu kayıt varsa migration hata verip durur. Zarf ve gönderimlerdeki
    /// kurum bilgisi boşaltılır, dış kurum personeli (ExternalUser) kayıtları silinir (ComAgency kurumlarına
    /// bağlı olarak yeniden girilir).
    /// </summary>
    public partial class UseComAgencyForExternalInstitutions : Migration
    {
        private static readonly (string Table, string Column, bool Nullable, string Index)[] InstitutionColumns =
        {
            ("IncomingDocuments", "ExternalInstitutionId", true, "IX_IncomingDocuments_ExternalInstitutionId"),
            ("OutgoingDocuments", "ExternalInstitutonId", true, "IX_OutgoingDocuments_ExternalInstitutonId"),
            ("OutgoingDocumentDistributions", "ExternalInstitutionId", true, "IX_OutgoingDocumentDistributions_ExternalInstitutionId"),
            ("OutgoingDocumentShipments", "ExternalInstitutionId", true, "IX_OutgoingDocumentShipments_ExternalInstitutionId"),
            ("ExternalUser", "ExternalInstitutionId", false, "IX_ExternalUser_ExternalInstitutionId"),
            ("Envelopes", "ExternalInstitutionId", true, null)
        };

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF EXISTS (SELECT 1 FROM IncomingDocuments WHERE ExternalInstitutionId IS NOT NULL)
   OR EXISTS (SELECT 1 FROM OutgoingDocuments WHERE ExternalInstitutonId IS NOT NULL)
   OR EXISTS (SELECT 1 FROM OutgoingDocumentDistributions WHERE ExternalInstitutionId IS NOT NULL)
    THROW 50002, N'Evraklarda eski dış kurum (ExternalInstitutions) bağları var; ComAgency eşlemesi yapılmadan bu migration uygulanamaz.', 1;");

            migrationBuilder.DropForeignKey(
                name: "FK_ExternalUser_ExternalInstitutions_ExternalInstitutionId",
                table: "ExternalUser");

            migrationBuilder.DropForeignKey(
                name: "FK_IncomingDocuments_ExternalInstitutions_ExternalInstitutionId",
                table: "IncomingDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_OutgoingDocumentDistributions_ExternalInstitutions_ExternalInstitutionId",
                table: "OutgoingDocumentDistributions");

            migrationBuilder.DropForeignKey(
                name: "FK_OutgoingDocuments_ExternalInstitutions_ExternalInstitutonId",
                table: "OutgoingDocuments");

            migrationBuilder.DropForeignKey(
                name: "FK_OutgoingDocumentShipments_ExternalInstitutions_ExternalInstitutionId",
                table: "OutgoingDocumentShipments");

            migrationBuilder.DropTable(
                name: "ExternalInstitutions");

            // Eski kurumlara bağlı dış kurum personeli taşınamaz; ComAgency kurumlarına bağlı olarak yeniden girilir
            migrationBuilder.Sql("DELETE FROM ExternalUser;");

            foreach (var (table, column, nullable, index) in InstitutionColumns)
            {
                if (index != null)
                    migrationBuilder.DropIndex(name: index, table: table);

                migrationBuilder.DropColumn(name: column, table: table);

                migrationBuilder.AddColumn<int>(
                    name: column,
                    table: table,
                    type: "int",
                    nullable: nullable,
                    defaultValue: nullable ? null : 0);

                if (index != null)
                    migrationBuilder.CreateIndex(name: index, table: table, column: column);
            }

            // Dışişleri'nin kendi birimleri (24305112 = DISISLERI BAKANLIGI ve altı) ve Diplomatik Portal'daki grup
            // düğümleri ("Büyükelçilikler" gibi alt kaydı olan DataSourceId 2 kayıtları) dış kurum sayılmaz.
            // Tür: DataSourceId 2 -> 1 misyon, 3 -> 4 uluslararası kuruluş, diğerleri -> 2 kurum; bkz. ExternalInstitutionTypes.
            // DETSİS numarası, DETSİS kaynaklı (DataSourceId 1) kayıtlarda 8 haneli AgencyId'dir.
            // "Dilekçe / Talep" ComAgency'de yok; -1 id'li sabit satır (ExternalInstitutionIds.Petition).
            // CREATE VIEW kendi batch'inde olmalı; idempotent betikte IF bloğuna sarıldığı için EXEC ile çalıştırılır.
            migrationBuilder.Sql(@"
EXEC(N'CREATE VIEW vw_ExternalInstitutions AS
WITH Mfa AS (
    SELECT AgencyId, 0 AS Lvl FROM ComAgency WHERE AgencyId = 24305112
    UNION ALL
    SELECT c.AgencyId, Mfa.Lvl + 1 FROM ComAgency c JOIN Mfa ON c.ParentAgencyId = Mfa.AgencyId WHERE Mfa.Lvl < 20
)
SELECT a.AgencyId       AS Id,
       a.ParentAgencyId AS ParentId,
       a.Name,
       a.Address,
       CASE WHEN a.DataSourceId = 1 AND a.AgencyId BETWEEN 10000000 AND 99999999
            THEN CAST(a.AgencyId AS nvarchar(8)) END AS DetsisCode,
       CASE a.DataSourceId WHEN 2 THEN 1 WHEN 3 THEN 4 ELSE 2 END AS Type,
       CAST(CASE WHEN a.IsDeleted = 1 OR a.IsActive = 0 THEN 1 ELSE 0 END AS bit) AS IsDeleted
FROM ComAgency a
WHERE NOT EXISTS (SELECT 1 FROM Mfa WHERE Mfa.AgencyId = a.AgencyId)
  AND NOT (a.DataSourceId = 2 AND EXISTS (SELECT 1 FROM ComAgency c WHERE c.ParentAgencyId = a.AgencyId))
UNION ALL
SELECT -1, NULL, N''Dilekçe / Talep'', NULL, NULL, 3, CAST(0 AS bit)');");
        }

        /// <inheritdoc />
        /// <remarks>
        /// Şemayı geri alır, veriyi değil: ExternalInstitutions boş oluşur, kurum bağları boş kalır.
        /// </remarks>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS vw_ExternalInstitutions;");
            migrationBuilder.Sql("DELETE FROM ExternalUser;");

            foreach (var (table, column, nullable, index) in InstitutionColumns)
            {
                if (index != null)
                    migrationBuilder.DropIndex(name: index, table: table);

                migrationBuilder.DropColumn(name: column, table: table);

                migrationBuilder.AddColumn<Guid>(
                    name: column,
                    table: table,
                    type: "uniqueidentifier",
                    nullable: nullable,
                    defaultValue: nullable ? null : Guid.Empty);

                if (index != null)
                    migrationBuilder.CreateIndex(name: index, table: table, column: column);
            }

            migrationBuilder.CreateTable(
                name: "ExternalInstitutions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DetsisCode = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalInstitutions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExternalInstitutions_ExternalInstitutions_ParentId",
                        column: x => x.ParentId,
                        principalTable: "ExternalInstitutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExternalInstitutions_DetsisCode",
                table: "ExternalInstitutions",
                column: "DetsisCode");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalInstitutions_ParentId",
                table: "ExternalInstitutions",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExternalUser_ExternalInstitutions_ExternalInstitutionId",
                table: "ExternalUser",
                column: "ExternalInstitutionId",
                principalTable: "ExternalInstitutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IncomingDocuments_ExternalInstitutions_ExternalInstitutionId",
                table: "IncomingDocuments",
                column: "ExternalInstitutionId",
                principalTable: "ExternalInstitutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OutgoingDocumentDistributions_ExternalInstitutions_ExternalInstitutionId",
                table: "OutgoingDocumentDistributions",
                column: "ExternalInstitutionId",
                principalTable: "ExternalInstitutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OutgoingDocuments_ExternalInstitutions_ExternalInstitutonId",
                table: "OutgoingDocuments",
                column: "ExternalInstitutonId",
                principalTable: "ExternalInstitutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OutgoingDocumentShipments_ExternalInstitutions_ExternalInstitutionId",
                table: "OutgoingDocumentShipments",
                column: "ExternalInstitutionId",
                principalTable: "ExternalInstitutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
