using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DijitalEvrakTakip.Persistance.Migrations
{
    /// <summary>
    /// ComTitle, kaynaktan gelen gerçek şemaya (EGovernmentCmsCommon.dbo.ComTitle) hizalanır: uzunluklar 180 / 30,
    /// metin kolonları Turkish_CI_AS; DefaultSortOrder, Code, TransferId, IsExternal, Trash eklenir. Veri
    /// degisiklik_notlari/comtitle_aktarimi.sql ile yüklenir (DISNET'ten doldurulan geçici 35 satırın yerine).
    /// </summary>
    public partial class AlignComTitleWithSource : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DefaultShortName",
                table: "ComTitle",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true,
                collation: "Turkish_CI_AS",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DefaultName",
                table: "ComTitle",
                type: "nvarchar(180)",
                maxLength: 180,
                nullable: false,
                collation: "Turkish_CI_AS",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "ComTitle",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                collation: "Turkish_CI_AS");

            migrationBuilder.AddColumn<short>(
                name: "DefaultSortOrder",
                table: "ComTitle",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsExternal",
                table: "ComTitle",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TransferId",
                table: "ComTitle",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Trash",
                table: "ComTitle",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "ComTitle");

            migrationBuilder.DropColumn(
                name: "DefaultSortOrder",
                table: "ComTitle");

            migrationBuilder.DropColumn(
                name: "IsExternal",
                table: "ComTitle");

            migrationBuilder.DropColumn(
                name: "TransferId",
                table: "ComTitle");

            migrationBuilder.DropColumn(
                name: "Trash",
                table: "ComTitle");

            migrationBuilder.AlterColumn<string>(
                name: "DefaultShortName",
                table: "ComTitle",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30,
                oldNullable: true,
                oldCollation: "Turkish_CI_AS");

            migrationBuilder.AlterColumn<string>(
                name: "DefaultName",
                table: "ComTitle",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(180)",
                oldMaxLength: 180,
                oldCollation: "Turkish_CI_AS");
        }
    }
}
