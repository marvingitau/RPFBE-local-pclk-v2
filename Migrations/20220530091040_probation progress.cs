using Microsoft.EntityFrameworkCore.Migrations;

namespace RPFBE.Migrations
{
    public partial class probationprogress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProbationProgress",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UID = table.Column<string>(nullable: true),
                    UIDComment = table.Column<string>(nullable: true),
                    UIDTwo = table.Column<string>(nullable: true),
                    UIDTwoComment = table.Column<string>(nullable: true),
                    UIDThree = table.Column<string>(nullable: true),
                    UIDThreeComment = table.Column<string>(nullable: true),
                    ProbationStatus = table.Column<int>(nullable: false),
                    ProbationNo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProbationProgress", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProbationProgress");
        }
    }
}
