using Microsoft.EntityFrameworkCore.Migrations;

namespace RPFBE.Migrations
{
    public partial class hrandfinhodfields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountantOne",
                table: "EmployeeClearance",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountantTwo",
                table: "EmployeeClearance",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "AnnualDaysLess",
                table: "EmployeeClearance",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "AnnualLeaveDays",
                table: "EmployeeClearance",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "BalDays",
                table: "EmployeeClearance",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<string>(
                name: "FinanceDirector",
                table: "EmployeeClearance",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FinanceDirectorName",
                table: "EmployeeClearance",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FinanceManager",
                table: "EmployeeClearance",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FinanceManagerName",
                table: "EmployeeClearance",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "JitSavings",
                table: "EmployeeClearance",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<string>(
                name: "NameOne",
                table: "EmployeeClearance",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameTwo",
                table: "EmployeeClearance",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "OtherLoan",
                table: "EmployeeClearance",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "StaffLoan",
                table: "EmployeeClearance",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<string>(
                name: "Year",
                table: "EmployeeClearance",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountantOne",
                table: "EmployeeClearance");

            migrationBuilder.DropColumn(
                name: "AccountantTwo",
                table: "EmployeeClearance");

            migrationBuilder.DropColumn(
                name: "AnnualDaysLess",
                table: "EmployeeClearance");

            migrationBuilder.DropColumn(
                name: "AnnualLeaveDays",
                table: "EmployeeClearance");

            migrationBuilder.DropColumn(
                name: "BalDays",
                table: "EmployeeClearance");

            migrationBuilder.DropColumn(
                name: "FinanceDirector",
                table: "EmployeeClearance");

            migrationBuilder.DropColumn(
                name: "FinanceDirectorName",
                table: "EmployeeClearance");

            migrationBuilder.DropColumn(
                name: "FinanceManager",
                table: "EmployeeClearance");

            migrationBuilder.DropColumn(
                name: "FinanceManagerName",
                table: "EmployeeClearance");

            migrationBuilder.DropColumn(
                name: "JitSavings",
                table: "EmployeeClearance");

            migrationBuilder.DropColumn(
                name: "NameOne",
                table: "EmployeeClearance");

            migrationBuilder.DropColumn(
                name: "NameTwo",
                table: "EmployeeClearance");

            migrationBuilder.DropColumn(
                name: "OtherLoan",
                table: "EmployeeClearance");

            migrationBuilder.DropColumn(
                name: "StaffLoan",
                table: "EmployeeClearance");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "EmployeeClearance");
        }
    }
}
