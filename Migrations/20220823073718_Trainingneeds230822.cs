using Microsoft.EntityFrameworkCore.Migrations;

namespace RPFBE.Migrations
{
    public partial class Trainingneeds230822 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TrainingNeedList",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EName = table.Column<string>(nullable: true),
                    EID = table.Column<string>(nullable: true),
                    Stage = table.Column<int>(nullable: false),
                    StageName = table.Column<string>(nullable: true),
                    HODComment = table.Column<string>(nullable: true),
                    HRComment = table.Column<string>(nullable: true),
                    FMDComment = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingNeedList", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrainingNeedList");
        }
    }
}
