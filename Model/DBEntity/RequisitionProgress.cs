using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Model.DBEntity
{
    public class RequisitionProgress
    {
        public int Id { get; set; }
        public string UID { get; set; }
        public string UIDComment { get; set; } //Immediate
        public string UIDTwo { get; set; } //HR
        public string UIDTwoComment { get; set; }
        public string UIDThree { get; set; } //MD
        public string UIDThreeComment { get; set; }
        public string UIDFour { get; set; }
        public string UIDFourComment { get; set; } //HOD
        public string ReqID { get; set; }
        public string JobNo { get; set; }
        public string JobTitle { get; set; }
        public string JobGrade { get; set; }
        public string RequestedEmployees { get; set; }
        public string ClosingDate { get; set; }
        public string Status { get; set; }
        public int ProgressStatus { get; set; } = 0; //Immediate Sup=0  HOD Level=1

        public string HodEID { get; set; }
        
    }
}
