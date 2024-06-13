using Microsoft.EntityFrameworkCore.Migrations;

namespace RPFBE.Migrations
{
    public partial class probationprogressv1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreationDate",
                table: "ProbationProgress",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "ProbationProgress",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmpID",
                table: "ProbationProgress",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmpName",
                table: "ProbationProgress",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImportantSkills",
                table: "ProbationProgress",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MgrID",
                table: "ProbationProgress",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Position",
                table: "ProbationProgress",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "ProbationProgress",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SupervisionTime",
                table: "ProbationProgress",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "ProbationProgress");

            migrationBuilder.DropColumn(
                name: "Department",
                table: "ProbationProgress");

            migrationBuilder.DropColumn(
                name: "EmpID",
                table: "ProbationProgress");

            migrationBuilder.DropColumn(
                name: "EmpName",
                table: "ProbationProgress");

            migrationBuilder.DropColumn(
                name: "ImportantSkills",
                table: "ProbationProgress");

            migrationBuilder.DropColumn(
                name: "MgrID",
                table: "ProbationProgress");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "ProbationProgress");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ProbationProgress");

            migrationBuilder.DropColumn(
                name: "SupervisionTime",
                table: "ProbationProgress");
        }
    }
}
