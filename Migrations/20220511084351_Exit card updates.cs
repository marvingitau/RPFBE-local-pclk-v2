using Microsoft.EntityFrameworkCore.Migrations;

namespace RPFBE.Migrations
{
    public partial class Exitcardupdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Division",
                table: "ExitInterviewCard",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmployeeName",
                table: "ExitInterviewCard",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExitNo",
                table: "ExitInterviewCard",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JobTitle",
                table: "ExitInterviewCard",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LengthOfService",
                table: "ExitInterviewCard",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherPositionsHeld",
                table: "ExitInterviewCard",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PositionStartDate",
                table: "ExitInterviewCard",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StartDateWithOrganization",
                table: "ExitInterviewCard",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Division",
                table: "ExitInterviewCard");

            migrationBuilder.DropColumn(
                name: "EmployeeName",
                table: "ExitInterviewCard");

            migrationBuilder.DropColumn(
                name: "ExitNo",
                table: "ExitInterviewCard");

            migrationBuilder.DropColumn(
                name: "JobTitle",
                table: "ExitInterviewCard");

            migrationBuilder.DropColumn(
                name: "LengthOfService",
                table: "ExitInterviewCard");

            migrationBuilder.DropColumn(
                name: "OtherPositionsHeld",
                table: "ExitInterviewCard");

            migrationBuilder.DropColumn(
                name: "PositionStartDate",
                table: "ExitInterviewCard");

            migrationBuilder.DropColumn(
                name: "StartDateWithOrganization",
                table: "ExitInterviewCard");
        }
    }
}
