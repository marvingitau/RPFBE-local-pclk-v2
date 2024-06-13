using Microsoft.EntityFrameworkCore.Migrations;

namespace RPFBE.Migrations
{
    public partial class monitoringtable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PerformanceMonitoring",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HODId = table.Column<string>(nullable: true),
                    HRId = table.Column<string>(nullable: true),
                    Progresscode = table.Column<int>(nullable: false),
                    PerformanceId = table.Column<string>(nullable: true),
                    StaffName = table.Column<string>(nullable: true),
                    ManagerName = table.Column<string>(nullable: true),
                    Date = table.Column<string>(nullable: true),
                    ApprovalStatus = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerformanceMonitoring", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PerformanceMonitoring");
        }
    }
}
