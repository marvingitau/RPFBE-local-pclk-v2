using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Model.Performance
{
    public class EmployeeAppraisalList
    {
        public int Id { get; set; }
        public string No { get; set; }
        public string EmployeeNo { get; set; }
        public string KPICode { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeDesgnation { get; set; }
        public string JobTitle { get; set; }
        public string ManagerNo { get; set; }
        public string ManagerName { get; set; }
        public string ManagerDesignation { get; set; }
        public string AppraisalPeriod { get; set; }
        public string AppraisalStartPeriod { get; set; }
        public string AppraisalEndPeriod { get; set; }
        public string AppraisalLevel { get; set; }
    }
}
