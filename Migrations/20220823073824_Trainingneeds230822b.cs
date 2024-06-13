using Microsoft.EntityFrameworkCore.Migrations;

namespace RPFBE.Migrations
{
    public partial class Trainingneeds230822b : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NeedNo",
                table: "TrainingNeedList",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NeedNo",
                table: "TrainingNeedList");
        }
    }
}
