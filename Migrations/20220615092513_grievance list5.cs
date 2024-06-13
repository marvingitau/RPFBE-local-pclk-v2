using Microsoft.EntityFrameworkCore.Migrations;

namespace RPFBE.Migrations
{
    public partial class grievancelist5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Resolver",
                table: "GrievanceList",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResolverID",
                table: "GrievanceList",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Resolver",
                table: "GrievanceList");

            migrationBuilder.DropColumn(
                name: "ResolverID",
                table: "GrievanceList");
        }
    }
}
