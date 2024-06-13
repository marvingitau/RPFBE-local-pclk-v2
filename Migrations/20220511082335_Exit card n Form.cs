using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RPFBE.Migrations
{
    public partial class ExitcardnForm : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExitInterviewCard",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UID = table.Column<string>(nullable: true),
                    EID = table.Column<string>(nullable: true),
                    InterviewDate = table.Column<DateTime>(nullable: false),
                    Interviewer = table.Column<string>(nullable: true),
                    SeparationGround = table.Column<string>(nullable: true),
                    OtherReason = table.Column<string>(nullable: true),
                    SeparationDate = table.Column<DateTime>(nullable: false),
                    Reemploy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExitInterviewCard", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExitInterviewForm",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UID = table.Column<string>(nullable: true),
                    Typeofwork = table.Column<string>(nullable: true),
                    Workingcondition = table.Column<string>(nullable: true),
                    Payment = table.Column<string>(nullable: true),
                    Manager = table.Column<string>(nullable: true),
                    Fairnessofworkload = table.Column<string>(nullable: true),
                    Salary = table.Column<string>(nullable: true),
                    WorkingconditionOne = table.Column<string>(nullable: true),
                    Toolsprovided = table.Column<string>(nullable: true),
                    Trainingreceived = table.Column<string>(nullable: true),
                    Rxtioncoworker = table.Column<string>(nullable: true),
                    Typeworkperformed = table.Column<string>(nullable: true),
                    Supervisonreceived = table.Column<string>(nullable: true),
                    Decisionaffected = table.Column<string>(nullable: true),
                    Recruitmentprocess = table.Column<string>(nullable: true),
                    Employeeorientation = table.Column<string>(nullable: true),
                    Trainingopportunity = table.Column<string>(nullable: true),
                    Careerdevops = table.Column<string>(nullable: true),
                    Employeemorale = table.Column<string>(nullable: true),
                    Fairtreatment = table.Column<string>(nullable: true),
                    Recognitionofwelldone = table.Column<string>(nullable: true),
                    Suportofworklifebal = table.Column<string>(nullable: true),
                    Cooperationinoffice = table.Column<string>(nullable: true),
                    Communicationmgtemp = table.Column<string>(nullable: true),
                    Performancedevplan = table.Column<string>(nullable: true),
                    Interestinvemp = table.Column<string>(nullable: true),
                    CommitmentCustServ = table.Column<string>(nullable: true),
                    ConcernedQualityExcellence = table.Column<string>(nullable: true),
                    AdminPolicy = table.Column<string>(nullable: true),
                    RecognitionAccomp = table.Column<string>(nullable: true),
                    ClearlyCommExpectation = table.Column<string>(nullable: true),
                    TreatedFairly = table.Column<string>(nullable: true),
                    CoarchedTrainedDev = table.Column<string>(nullable: true),
                    ProvidedLeadership = table.Column<string>(nullable: true),
                    EncouragedTeamworkCoop = table.Column<string>(nullable: true),
                    ResolvedConcerns = table.Column<string>(nullable: true),
                    ListeningToSuggetions = table.Column<string>(nullable: true),
                    KeptTeamInfo = table.Column<string>(nullable: true),
                    SupportedWorkLifeBal = table.Column<string>(nullable: true),
                    AppropriateChallengingAssignments = table.Column<string>(nullable: true),
                    Whatulldosummarydous = table.Column<string>(nullable: true),
                    TheJobLeaving = table.Column<string>(nullable: true),
                    TheOrgoverla = table.Column<string>(nullable: true),
                    YourSupervisorMgr = table.Column<string>(nullable: true),
                    AnyOtherSuggetionQ = table.Column<string>(nullable: true),
                    NowDate = table.Column<DateTime>(nullable: false),
                    ExitCardRef = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExitInterviewForm", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExitInterviewForm_ExitInterviewCard_ExitCardRef",
                        column: x => x.ExitCardRef,
                        principalTable: "ExitInterviewCard",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExitInterviewForm_ExitCardRef",
                table: "ExitInterviewForm",
                column: "ExitCardRef",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExitInterviewForm");

            migrationBuilder.DropTable(
                name: "ExitInterviewCard");
        }
    }
}
