using Microsoft.EntityFrameworkCore.Migrations;

namespace RPFBE.Migrations
{
    public partial class probationprogress1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImmediateManagerComment",
                table: "ProbationProgress",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImmediateManagerID",
                table: "ProbationProgress",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImmediateManagerComment",
                table: "ProbationProgress");

            migrationBuilder.DropColumn(
                name: "ImmediateManagerID",
                table: "ProbationProgress");
        }
    }
}
