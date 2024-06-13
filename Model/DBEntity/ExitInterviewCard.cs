using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Model.DBEntity
{
    public class ExitInterviewCard
    {
        public int Id { get; set; }
        public string UID { get; set; }
        public string EID { get; set; }
        public DateTime InterviewDate { get; set; }
        public string Interviewer { get; set; }
        public string SeparationGround { get; set; }
        public string OtherReason { get; set; }
        public DateTime SeparationDate { get; set; }
        public string Reemploy { get; set; }
        public string ExitNo { get; set; }
        public string EmployeeName { get; set; }
        public string JobTitle { get; set; }
        public string Division { get; set; }
        public string StartDateWithOrganization { get; set; }
        public string PositionStartDate { get; set; }
        public string LengthOfService { get; set; }
        public string OtherPositionsHeld { get; set; }

        public int FormUploaded { get; set; } = 0;


       // public ExitInterviewForm ExitInterviewForm { get; set; }


    }
}
