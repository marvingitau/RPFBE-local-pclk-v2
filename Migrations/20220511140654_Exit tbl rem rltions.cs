using Microsoft.EntityFrameworkCore.Migrations;

namespace RPFBE.Migrations
{
    public partial class Exittblremrltions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExitInterviewForm_ExitInterviewCard_ExitCardRef",
                table: "ExitInterviewForm");

            migrationBuilder.DropIndex(
                name: "IX_ExitInterviewForm_ExitCardRef",
                table: "ExitInterviewForm");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ExitInterviewForm_ExitCardRef",
                table: "ExitInterviewForm",
                column: "ExitCardRef",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ExitInterviewForm_ExitInterviewCard_ExitCardRef",
                table: "ExitInterviewForm",
                column: "ExitCardRef",
                principalTable: "ExitInterviewCard",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
