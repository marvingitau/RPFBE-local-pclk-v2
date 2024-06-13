using Microsoft.EntityFrameworkCore.Migrations;

namespace RPFBE.Migrations
{
    public partial class monitoringtables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MonitoringDoc",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UID = table.Column<string>(nullable: true),
                    MonitoringID = table.Column<string>(nullable: true),
                    Filename = table.Column<string>(nullable: true),
                    Filepath = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonitoringDoc", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MonitoringSupportingDoc",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MonitorId = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    FilePath = table.Column<string>(nullable: true),
                    TagName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonitoringSupportingDoc", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MonitoringDocView",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<string>(nullable: true),
                    Viewed = table.Column<string>(nullable: true),
                    MonitoringDocId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonitoringDocView", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MonitoringDocView_MonitoringDoc_MonitoringDocId",
                        column: x => x.MonitoringDocId,
                        principalTable: "MonitoringDoc",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MonitoringDocView_MonitoringDocId",
                table: "MonitoringDocView",
                column: "MonitoringDocId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MonitoringDocView");

            migrationBuilder.DropTable(
                name: "MonitoringSupportingDoc");

            migrationBuilder.DropTable(
                name: "MonitoringDoc");
        }
    }
}
