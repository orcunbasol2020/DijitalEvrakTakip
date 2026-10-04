using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DijitalEvrakTakip.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddComAgencyAndComDepartment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ComAgency",
                columns: table => new
                {
                    AgencyId = table.Column<int>(type: "int", nullable: false),
                    ParentAgencyId = table.Column<int>(type: "int", nullable: true),
                    DepartmentId = table.Column<int>(type: "int", nullable: true),
                    AgencyCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MainCode = table.Column<string>(type: "char(2)", unicode: false, fixedLength: true, maxLength: 2, nullable: true),
                    DepartmentCode = table.Column<string>(type: "nchar(3)", fixedLength: true, maxLength: 3, nullable: true),
                    SubAgencyCode1 = table.Column<string>(type: "char(2)", unicode: false, fixedLength: true, maxLength: 2, nullable: true),
                    SubAgencyCode2 = table.Column<string>(type: "char(2)", unicode: false, fixedLength: true, maxLength: 2, nullable: true),
                    SubAgencyCode3 = table.Column<string>(type: "char(2)", unicode: false, fixedLength: true, maxLength: 2, nullable: true),
                    SubAgencyCode4 = table.Column<string>(type: "char(2)", unicode: false, fixedLength: true, maxLength: 2, nullable: true),
                    DistributionCode1 = table.Column<string>(type: "char(1)", unicode: false, fixedLength: true, maxLength: 1, nullable: true),
                    DistributionCode2 = table.Column<string>(type: "char(1)", unicode: false, fixedLength: true, maxLength: 1, nullable: true),
                    DistributionCode3 = table.Column<string>(type: "char(1)", unicode: false, fixedLength: true, maxLength: 1, nullable: true),
                    DistributionCode4 = table.Column<string>(type: "char(1)", unicode: false, fixedLength: true, maxLength: 1, nullable: true),
                    HasSubAgency = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Name = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Fax = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    TypeCode = table.Column<string>(type: "char(1)", unicode: false, fixedLength: true, maxLength: 1, nullable: true),
                    WebAddress = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    KEPAddress = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ECorrespondenceUsage = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    ECorrespondenceEndPoint = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    DataSourceId = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    OldId = table.Column<int>(type: "int", nullable: true),
                    IsCacheable = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ECorrespondenceName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    CanSendECorrespondence = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    ECorrespondenceType = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ParentAdminAgencyId = table.Column<int>(type: "int", nullable: true),
                    PathFlaten = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    DistributionTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Position = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CorrespondenceAgencyId = table.Column<int>(type: "int", nullable: true, comment: "Dolu ise ,yazışma burada yazan birim ile yapılabilir. Seçili birim ile direkt yazışma yapılamıyor demekki."),
                    CorrespondenceAgencyText = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "Yazışma birim metni"),
                    UseCorrespondenceAgencyIdAsAgencyId = table.Column<bool>(type: "bit", nullable: true, comment: "Bazı durumlarda, agencyId seçip, yazıda CorrespondenceAgency deki Name kullanılabilir."),
                    KurumTip = table.Column<int>(type: "int", nullable: true),
                    CorrespondenceAgencyTextParent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DistributionTitleParent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Test = table.Column<long>(type: "bigint", nullable: true),
                    MersisNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComAgency", x => x.AgencyId);
                });

            migrationBuilder.CreateTable(
                name: "ComDepartment",
                columns: table => new
                {
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    ParentDepartmentId = table.Column<int>(type: "int", nullable: true),
                    CountryId = table.Column<int>(type: "int", nullable: false),
                    DepartmentTypeId = table.Column<int>(type: "int", nullable: true),
                    Code = table.Column<string>(type: "char(4)", unicode: false, fixedLength: true, maxLength: 4, nullable: true),
                    DefaultName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DefaultShortName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    TransferId = table.Column<int>(type: "int", nullable: true),
                    IsExternal = table.Column<bool>(type: "bit", nullable: true),
                    Trash = table.Column<bool>(type: "bit", nullable: true),
                    DutyArea = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDomisticOrganization = table.Column<bool>(type: "bit", nullable: false),
                    DomisticOrganizationSortOrder = table.Column<int>(type: "int", nullable: true),
                    DepartmentLevel = table.Column<int>(type: "int", nullable: true),
                    RegionClass = table.Column<int>(type: "int", nullable: true),
                    CamLink = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CountyId = table.Column<int>(type: "int", nullable: true),
                    CityId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComDepartment", x => x.DepartmentId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComAgency_DepartmentId",
                table: "ComAgency",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ComAgency_ParentAgencyId",
                table: "ComAgency",
                column: "ParentAgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_ComDepartment_ParentDepartmentId",
                table: "ComDepartment",
                column: "ParentDepartmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComAgency");

            migrationBuilder.DropTable(
                name: "ComDepartment");
        }
    }
}
