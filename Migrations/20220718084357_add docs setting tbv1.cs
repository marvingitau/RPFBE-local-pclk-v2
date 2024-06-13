using Microsoft.EntityFrameworkCore.Migrations;

namespace RPFBE.Migrations
{
    public partial class adddocssettingtbv1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReadMandatoty",
                table: "DocumentSetting");

            migrationBuilder.AddColumn<string>(
                name: "LastUser",
                table: "DocumentSetting",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReadMandatory",
                table: "DocumentSetting",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastUser",
                table: "DocumentSetting");

            migrationBuilder.DropColumn(
                name: "ReadMandatory",
                table: "DocumentSetting");

            migrationBuilder.AddColumn<string>(
                name: "ReadMandatoty",
                table: "DocumentSetting",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
