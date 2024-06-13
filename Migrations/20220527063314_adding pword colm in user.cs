using Microsoft.EntityFrameworkCore.Migrations;

namespace RPFBE.Migrations
{
    public partial class addingpwordcolminuser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Pcode",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pcode",
                table: "AspNetUsers");
        }
    }
}
