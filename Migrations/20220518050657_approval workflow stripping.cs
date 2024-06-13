using Microsoft.EntityFrameworkCore.Migrations;

namespace RPFBE.Migrations
{
    public partial class approvalworkflowstripping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HODAdminApproved",
                table: "EmployeeClearance",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HODAdminApprovedName",
                table: "EmployeeClearance",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HODAdminApprovedUID",
                table: "EmployeeClearance",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HODApproved",
                table: "EmployeeClearance",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HODApprovedName",
                table: "EmployeeClearance",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HODApprovedUID",
                table: "EmployeeClearance",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HODFINApproved",
                table: "EmployeeClearance",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HODFINApprovedName",
                table: "EmployeeClearance",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HODFINApprovedUID",
                table: "EmployeeClearance",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HODHRApproved",
                table: "EmployeeClearance",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HODHRApprovedName",
                table: "EmployeeClearance",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HODHRApprovedUID",
                table: "EmployeeClearance",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HODITApproved",
                table: "EmployeeClearance",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HODITApprovedName",
                table: "EmployeeClearance",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HODITApprovedUID",
                table: "EmployeeClearance",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HRApproved",
                table: "EmployeeClearance",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HRApprovedName",
                table: "EmployeeClearance",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HRApprovedUID",
                table: "EmployeeClearance",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HODAdminApproved",
                table: "EmployeeClearance");

            migrationBuilder.DropColumn(
                name: "HODAdminApprovedName",
                table: "EmployeeClearance");

            migrationBuilder.DropColumn(
                name: "HODAdminApprovedUID",
                table: "EmployeeClearance");

            migrationBuilder.DropColumn(
                name: "HODApproved",
                table: "EmployeeClearance");

            migrationBuilder.DropColumn(
                name: "HODApprovedName",
                table: "EmployeeClearance");

            migrationBuilder.DropColumn(
                name: "HODApprovedUID",
                table: "EmployeeClearance");

            migrationBuilder.DropColumn(
                name: "HODFINApproved",
                table: "EmployeeClearance");

            migrationBuilder.DropColumn(
                name: "HODFINApprovedName",
                table: "EmployeeClearance");

            migrationBuilder.DropColumn(
                name: "HODFINApprovedUID",
                table: "EmployeeClearance");

            migrationBuilder.DropColumn(
                name: "HODHRApproved",
                table: "EmployeeClearance");

            migrationBuilder.DropColumn(
                name: "HODHRApprovedName",
                table: "EmployeeClearance");

            migrationBuilder.DropColumn(
                name: "HODHRApprovedUID",
                table: "EmployeeClearance");

            migrationBuilder.DropColumn(
                name: "HODITApproved",
                table: "EmployeeClearance");

            migrationBuilder.DropColumn(
                name: "HODITApprovedName",
                table: "EmployeeClearance");

            migrationBuilder.DropColumn(
                name: "HODITApprovedUID",
                table: "EmployeeClearance");

            migrationBuilder.DropColumn(
                name: "HRApproved",
                table: "EmployeeClearance");

            migrationBuilder.DropColumn(
                name: "HRApprovedName",
                table: "EmployeeClearance");

            migrationBuilder.DropColumn(
                name: "HRApprovedUID",
                table: "EmployeeClearance");
        }
    }
}
