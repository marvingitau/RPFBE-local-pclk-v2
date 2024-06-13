using Microsoft.EntityFrameworkCore.Migrations;

namespace RPFBE.Migrations
{
    public partial class grievancelist2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "GrievanceList",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "GrievanceList",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GrievanceType",
                table: "GrievanceList",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Outcome",
                table: "GrievanceList",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Recommendation",
                table: "GrievanceList",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StepTaken",
                table: "GrievanceList",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "GrievanceList",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comment",
                table: "GrievanceList");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "GrievanceList");

            migrationBuilder.DropColumn(
                name: "GrievanceType",
                table: "GrievanceList");

            migrationBuilder.DropColumn(
                name: "Outcome",
                table: "GrievanceList");

            migrationBuilder.DropColumn(
                name: "Recommendation",
                table: "GrievanceList");

            migrationBuilder.DropColumn(
                name: "StepTaken",
                table: "GrievanceList");

            migrationBuilder.DropColumn(
                name: "Subject",
                table: "GrievanceList");
        }
    }
}
