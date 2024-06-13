using Microsoft.EntityFrameworkCore.Migrations;

namespace RPFBE.Migrations
{
    public partial class addedcalederlabel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Calender",
                table: "TrainingNeedList",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CalenderLabel",
                table: "TrainingNeedList",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Calender",
                table: "TrainingNeedList");

            migrationBuilder.DropColumn(
                name: "CalenderLabel",
                table: "TrainingNeedList");
        }
    }
}
