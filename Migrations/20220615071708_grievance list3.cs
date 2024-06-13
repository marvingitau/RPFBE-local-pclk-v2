using Microsoft.EntityFrameworkCore.Migrations;

namespace RPFBE.Migrations
{
    public partial class grievancelist3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Employeename",
                table: "GrievanceList",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Supervisorname",
                table: "GrievanceList",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Employeename",
                table: "GrievanceList");

            migrationBuilder.DropColumn(
                name: "Supervisorname",
                table: "GrievanceList");
        }
    }
}
