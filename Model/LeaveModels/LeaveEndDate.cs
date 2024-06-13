using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Model.LeaveModels
{
    public class LeaveEndDate
    {
        public int Id { get; set; }
        public string EmployeeNo { get; set; }
        public string LeaveType { get; set; }
        public DateTime LeaveStartDate { get; set; }
        public decimal DaysApplied { get; set; }
        public string LeaveAppNo { get; set; }
        public string RelieverNo { get; set; }
        public string RelieverRemark { get; set; }
        public string RejectionRemark { get; set; }

    }
}
