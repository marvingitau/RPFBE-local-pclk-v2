using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Model.LeaveModels
{
    public class LeaveApplicationList
    {
        public int Id { get; set; }
        public string No { get; set; }
        public string EmployeeNo { get; set; }
        public string EmployeeName { get; set; }
        public string LeaveType { get; set; }
        public string LeaveStartDate { get; set; }
        public string LeaveBalance { get; set; }
        public string DaysApplied { get; set; }
        public string DaysApproved { get; set; }
        public string LeaveEndDate { get; set; }
        public string LeaveReturnDate { get; set; }
        public string ReasonForLeave { get; set; }
        public string SubstituteEmployeeNo { get; set; }
        public string SubstituteEmployeeName { get; set; }
        public string GlobalDimension1Code { get; set; }
        public string GlobalDimension2Code { get; set; }
        public string ShortcutDimension3Code { get; set; }
        public string ShortcutDimension4Code { get; set; }
        public string ShortcutDimension5Code { get; set; }
        public string ShortcutDimension6Code { get; set; }
        public string ShortcutDimension7Code { get; set; }
        public string ShortcutDimension8Code { get; set; }
        public string ResponsibilityCenter { get; set; }
        public string RejectionComments { get; set; }
        public string Status { get; set; }
    }
}
