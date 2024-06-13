using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Model.Performance
{
    public class AppraisalTargetList
    {
        public int Id { get; set; }
        public string No { get; set; }
        public string EmployeeName { get; set; }
        public string Period { get; set; }
        public string EmployeeDesgnation { get; set; }
        public string JobNo { get; set; }
        public string JobTitle { get; set; }
        public string ManagerNo { get; set; }
        public string ManagerName { get; set; }
        public string ManagerDesignation { get; set; }
        public string AppraisalPeriod { get; set; }
        public string AppraisalStartPeriod { get; set; }
        public string AppraisalEndPeriod { get; set; }
        public string EmployeeWeightedScore { get; set; }
        public string SupervisorWeightedScore { get; set; }
        public string OverallWeightedScore { get; set; }
    }
}
