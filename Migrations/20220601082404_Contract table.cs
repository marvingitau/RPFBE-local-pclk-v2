using Microsoft.EntityFrameworkCore.Migrations;

namespace RPFBE.Migrations
{
    public partial class Contracttable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EndofContractProgress",
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
                    ContractStatus = table.Column<int>(nullable: false),
                    ContractNo = table.Column<string>(nullable: true),
                    EmpID = table.Column<string>(nullable: true),
                    MgrID = table.Column<string>(nullable: true),
                    MgrName = table.Column<string>(nullable: true),
                    SupervisionTime = table.Column<string>(nullable: true),
                    DoRenew = table.Column<string>(nullable: true),
                    EmpName = table.Column<string>(nullable: true),
                    CreationDate = table.Column<string>(nullable: true),
                    Department = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    Position = table.Column<string>(nullable: true),
                    RenewReason = table.Column<string>(nullable: true),
                    Howlong = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EndofContractProgress", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EndofContractProgress");
        }
    }
}
