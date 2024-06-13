using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Model.DBEntity
{
    public class ProbationProgress
    {
        public int Id { get; set; }
        public string UID { get; set; }//Immediate Sup
        public string UIDComment { get; set; }//Immediate Sup
        public string UIDTwo { get; set; }//HR
        public string UIDTwoComment { get; set; }//HR
        public string UIDThree { get; set; }//MD/FD
        public string UIDThreeComment { get; set; }//MD/FD
        public int ProbationStatus { get; set; } = 0;
        public string ProbationNo{ get; set; }

        public string EmpID { get; set; }
        public string MgrID { get; set; }
        public string MgrName { get; set; }
        public string SupervisionTime { get; set; }
        public string ImportantSkills { get; set; }
        public string EmpName { get; set; }
        public string CreationDate { get; set; }
        public string Department { get; set; }
        public string Status { get; set; }
        public string Position { get; set; }

        public string HODEid { get; set; }
        public string HODComment { get; set; }
        public string UIDFour { get; set; }//HOD - Bucketing option 
        public string UIDFourComment { get; set; }//HOD 
        public string BackTrackingReason { get; set; }
    }
}
