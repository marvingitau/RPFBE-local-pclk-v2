using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPFBE.Model.LeaveModels
{
    public class ExtraDaySingle
    {
        public string Leaveno { get; set; }
        public string Employeeno { get; set; }
        public DateTime Startdate { get; set; }
        public decimal Days { get; set; }
        public DateTime Enddate { get; set; }
        public DateTime Returndate { get; set; }
        public string Assignedleaveno { get; set; }
    }
}
