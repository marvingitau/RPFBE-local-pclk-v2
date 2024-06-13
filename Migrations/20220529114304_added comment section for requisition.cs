using Microsoft.EntityFrameworkCore.Migrations;

namespace RPFBE.Migrations
{
    public partial class addedcommentsectionforrequisition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UIDComment",
                table: "RequisitionProgress",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UIDFourComment",
                table: "RequisitionProgress",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UIDThreeComment",
                table: "RequisitionProgress",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UIDTwoComment",
                table: "RequisitionProgress",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UIDComment",
                table: "RequisitionProgress");

            migrationBuilder.DropColumn(
                name: "UIDFourComment",
                table: "RequisitionProgress");

            migrationBuilder.DropColumn(
                name: "UIDThreeComment",
                table: "RequisitionProgress");

            migrationBuilder.DropColumn(
                name: "UIDTwoComment",
                table: "RequisitionProgress");
        }
    }
}
