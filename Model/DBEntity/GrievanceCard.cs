using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Model.DBEntity
{
    public class GrievanceCard
    {
        public int Id { get; set; }
        public string GID { get; set; }
        public string EmpID { get; set; }
        public string Station { get; set; }
        public string Section { get; set; }
        public string Dept { get; set; }
        public string Supervisor { get; set; }
        public string CurrentStage { get; set; }
        public string NextStage { get; set; }
        public string GrievanceType { get; set; }
        public bool WorkEnv { get; set; }
        public bool EmployeeRln { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public string StepTaken { get; set; }
        public string Outcome { get; set; }
        public string Comment { get; set; }
        public string Recommendation { get; set; }
        public DateTime GrievanceDate { get; set; }
        public DateTime DateofIssue { get; set; }

        public string NextStageStaff { get; set; }
        public string GeneralRemark { get; set; }

        public string CycletwoInitreason { get; set; }
        public string Cycletwosteps { get; set; }
        public string Cycletwooutcome { get; set; }
        public string Cycletworecommendation { get; set; }

        public string AppealAlternativeRemark { get; set; }
        public string AppealOutcomeRemark { get; set; }


        public string ReverseOneEid { get; set; }
        public string ReverseOneReason { get; set; }
        public string ReverseThreeEid { get; set; }
        public string ReverseThreeReason { get; set; }

        public string StageStaff { get; set; }
        public string Stage { get; set; }
        public string ReverseReason { get; set; }


    }
}
