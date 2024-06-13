using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Model
{
    public class LeaveSubmanager
    {
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string ManagerId { get; set; }
        public string LeaveType { get; set; }
        public string LeaveDays { get; set; }
        public string LeaveStart { get; set; }
        public string LeaveEnd { get; set; }
        public List<LeaveSubmanagerEmployee> SubRows { get; set; }
    }
}
